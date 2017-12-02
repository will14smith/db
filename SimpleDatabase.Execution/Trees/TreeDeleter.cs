using System;
using SimpleDatabase.Storage;
using SimpleDatabase.Storage.Nodes;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;

namespace SimpleDatabase.Execution.Trees
{
    public class TreeDeleter
    {
        private readonly IPager _pager;
        private readonly IRowSerializer _rowSerializer;
        private readonly StoredTable _table;

        public TreeDeleter(IPager pager, IRowSerializer rowSerializer, StoredTable table)
        {
            _pager = pager;
            _rowSerializer = rowSerializer;
            _table = table;
        }

        public TreeDeleteResult Delete(int key)
        {
            var page = _pager.Get(_table.RootPageNumber);
            var node = Node.Read(_rowSerializer, page);

            Result result;
            switch (node)
            {
                case LeafNode leafNode:
                    result = LeafDelete(leafNode, key);
                    break;
                case InternalNode internalNode:
                    result = InternalDelete(internalNode, key);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (result)
            {
                case Result.Success _:
                    return new TreeDeleteResult.Success(key);

                case Result.KeyNotFound keyNotFound:
                    return new TreeDeleteResult.KeyNotFound(keyNotFound.Key);

                case Result.NodeUnderflow underflow:
                    PromoteSingleChildToRoot(page, (InternalNode)underflow.Node);
                    return new TreeDeleteResult.Success(key);

                default:
                    throw new ArgumentOutOfRangeException($"Unhandled result: {result}");
            }
        }

        private void PromoteSingleChildToRoot(Page page, InternalNode underflowNode)
        {
            if (!underflowNode.IsRoot)
            {
                throw new InvalidOperationException("Cannot promote from non-root");
            }
            if (underflowNode.KeyCount != 0)
            {
                throw new InvalidOperationException("There isn't a single child to promote.");
            }

            var childPageNumber = underflowNode.GetChild(0);
            var childPage = _pager.Get(childPageNumber);

            Array.Copy(childPage.Data, page.Data, Pager.PageSize);
            _pager.Free(childPageNumber);
        }

        private Result LeafDelete(LeafNode node, int key)
        {
            var result = LeafDeleteNoUnderflow(node, key);
            if (result is Result.KeyNotFound)
            {
                return result;
            }

            var leafIsValid = node.IsRoot || node.CellCount >= node.Layout.LeafNodeMinCells;
            if (leafIsValid)
            {
                return result;
            }

            return new Result.NodeUnderflow(node);
        }

        private Result LeafDeleteNoUnderflow(LeafNode node, int key)
        {
            var cellIndex = new TreeKeySearcher(key).FindCell(node);
            if (node.GetCellKey(cellIndex) != key)
            {
                return new Result.KeyNotFound(key);
            }

            for (var i = cellIndex + 1; i < node.CellCount; i++)
            {
                node.CopyCell(node, i, i - 1);
            }
            node.CellCount -= 1;

            return new Result.Success();
        }

        private Result InternalDelete(InternalNode internalNode, int key)
        {
            var childIndex = new TreeKeySearcher(key).FindCell(internalNode);
            var childPage = _pager.Get(internalNode.GetChild(childIndex));
            var childNode = Node.Read(_rowSerializer, childPage);

            Result childResult;
            switch (childNode)
            {
                case LeafNode leafChildNode:
                    childResult = LeafDelete(leafChildNode, key);
                    break;
                case InternalNode internalChildNode:
                    childResult = InternalDelete(internalChildNode, key);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (childResult)
            {
                case Result.Success _:
                case Result.KeyNotFound _:
                    return childResult;

                case Result.NodeUnderflow _:
                    return ResolveUnderflow(internalNode, childIndex, childNode);

                default:
                    throw new ArgumentOutOfRangeException($"Unhandled result: {childResult}");
            }
        }

        private void InternalDeleteNoUnderflow(InternalNode internalNode, int childIndex)
        {
            if (childIndex == internalNode.KeyCount)
            {
                internalNode.RightChild = internalNode.GetChild(internalNode.KeyCount - 1);
            }
            else
            {
                for (var i = childIndex; i < internalNode.KeyCount; i++)
                {
                    internalNode.CopyCell(internalNode, i + 1, i);
                }
            }

            internalNode.KeyCount -= 1;
        }

        private Result ResolveUnderflow(InternalNode internalNode, int childIndex, Node childNode)
        {
            var hasPrevSibling = childIndex > 0;
            var hasNextSibling = childIndex < internalNode.KeyCount;

            if (!hasPrevSibling && !hasNextSibling)
            {
                throw new InvalidOperationException("Cannot have a node with no siblings unless we are root (in which case this shouldn't have been called)");
            }

            var prevNode = hasPrevSibling ? Node.Read(_rowSerializer, _pager.Get(internalNode.GetChild(childIndex - 1))) : null;
            var nextNode = hasNextSibling ? Node.Read(_rowSerializer, _pager.Get(internalNode.GetChild(childIndex + 1))) : null;

            if (hasPrevSibling && HasMoreThanMinimumChildren(prevNode))
            {
                var newKey = BorrowFromPrev(childNode, prevNode);
                internalNode.SetKey(childIndex - 1, newKey);
                return new Result.Success();
            }

            if (hasNextSibling && HasMoreThanMinimumChildren(nextNode))
            {
                var newKey = BorrowFromNext(childNode, nextNode);
                internalNode.SetKey(childIndex, newKey);
                return new Result.Success();
            }

            if (hasPrevSibling)
            {
                Merge(prevNode, childNode);
                InternalDeleteNoUnderflow(internalNode, childIndex);
                _pager.Free(childNode.PageNumber);
            }
            else
            {
                Merge(childNode, nextNode);
                InternalDeleteNoUnderflow(internalNode, childIndex + 1);
                _pager.Free(nextNode.PageNumber);
            }

            var isValidInternalNode = (internalNode.IsRoot && internalNode.KeyCount > 0) || internalNode.KeyCount >= internalNode.Layout.InternalNodeMinCells;
            if (isValidInternalNode)
            {
                return new Result.Success();
            }

            return new Result.NodeUnderflow(internalNode);
        }

        private bool HasMoreThanMinimumChildren(Node node)
        {
            switch (node)
            {
                case LeafNode leafNode:
                    return leafNode.CellCount > node.Layout.LeafNodeMinCells;
                case InternalNode internalNode:
                    return internalNode.KeyCount > node.Layout.InternalNodeMinCells;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private int BorrowFromPrev(Node node, Node prevNode)
        {
            // move max(prevNode) -> start(node)

            switch (node)
            {
                case LeafNode leafNode:
                    var leafPrevNode = (LeafNode)prevNode;

                    leafNode.CellCount += 1;
                    for (var i = leafNode.CellCount - 1; i > 0; i--)
                    {
                        leafNode.CopyCell(leafNode, i - 1, i);
                    }
                    leafNode.CopyCell(leafPrevNode, leafPrevNode.CellCount - 1, 0);

                    leafPrevNode.CellCount -= 1;

                    return leafPrevNode.GetCellKey(leafPrevNode.CellCount - 1);
                case InternalNode internalNode:
                    throw new NotImplementedException("BorrowFromNext InternalNode");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        private int BorrowFromNext(Node node, Node nextNode)
        {
            // move min(nextNode) -> end(node)

            switch (node)
            {
                case LeafNode leafNode:
                    var leafNextNode = (LeafNode)nextNode;
                    var key = leafNextNode.GetCellKey(0);

                    leafNode.CellCount += 1;
                    leafNode.CopyCell(leafNextNode, 0, leafNode.CellCount - 1);

                    for (var i = 0; i < leafNextNode.CellCount; i++)
                    {
                        leafNextNode.CopyCell(leafNextNode, i + 1, i);
                    }
                    leafNextNode.CellCount -= 1;

                    return key;
                case InternalNode internalNode:
                    throw new NotImplementedException("BorrowFromNext InternalNode");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Merge(Node prevNode, Node node)
        {
            // move all from node -> prevNode

            switch (node)
            {
                case LeafNode leafNode:
                    var leafPrevNode = (LeafNode)prevNode;

                    var offset = leafPrevNode.CellCount;
                    leafPrevNode.CellCount += leafNode.CellCount;
                    for (var i = 0; i < leafNode.CellCount; i++)
                    {
                        leafPrevNode.CopyCell(leafNode, i, offset + i);
                    }

                    leafPrevNode.NextLeaf = leafNode.NextLeaf;
                    break;
                case InternalNode internalNode:
                    throw new NotImplementedException("Merge InternalNode");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private abstract class Result
        {
            public class Success : Result { }

            public class KeyNotFound : Result
            {
                public int Key { get; }

                public KeyNotFound(int key)
                {
                    Key = key;
                }
            }

            public class NodeUnderflow : Result
            {
                public Node Node { get; }

                public NodeUnderflow(Node node)
                {
                    Node = node;
                }
            }
        }
    }
}
