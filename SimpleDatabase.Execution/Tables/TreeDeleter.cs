using System;
using SimpleDatabase.Execution.Trees;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;
using SimpleDatabase.Storage.Tree;

namespace SimpleDatabase.Execution.Tables
{
    public class TreeDeleter
    {

        private readonly TableManager _tableManager;
        private readonly TableIndex _index;
        private readonly IIndexSerializer _serializer;

        public TreeDeleter(TableManager tableManager, TableIndex index)
        {
            _tableManager = tableManager;
            _index = index;
            _serializer = index.CreateSerializer();
        }

        public DeleteResult Delete(IndexKey key)
        {
            var page = _tableManager.Pager.Get(_tableManager.GetIndexRootPageId(_index));
            var node = Node.Read(page, _serializer);

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
                    return new DeleteResult.Success();

                case Result.KeyNotFound _:
                    return new DeleteResult.KeyNotFound();

                case Result.NodeUnderflow underflow:
                    PromoteSingleChildToRoot(page, (InternalNode)underflow.Node);
                    return new DeleteResult.Success();

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

            var pager = _tableManager.Pager;

            var childPageNumber = underflowNode.GetChild(0);
            var childPage = pager.Get(childPageNumber);

            Array.Copy(childPage.Data, page.Data, PageLayout.PageSize);
            pager.Free(childPageNumber);
        }

        private Result LeafDelete(LeafNode node, IndexKey key)
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

        private Result LeafDeleteNoUnderflow(LeafNode node, IndexKey key)
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

        private Result InternalDelete(InternalNode internalNode, IndexKey key)
        {
            var childIndex = new TreeKeySearcher(key).FindCell(internalNode);
            var childPage = _tableManager.Pager.Get(internalNode.GetChild(childIndex));
            var childNode = Node.Read(childPage, _serializer);

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

            var pager = _tableManager.Pager;
            
            var prevNode = hasPrevSibling ? Node.Read(pager.Get(internalNode.GetChild(childIndex - 1)), _serializer) : null;
            var nextNode = hasNextSibling ? Node.Read(pager.Get(internalNode.GetChild(childIndex + 1)), _serializer) : null;

            if (hasPrevSibling && HasMoreThanMinimumChildren(prevNode!))
            {
                var newKey = BorrowFromPrev(childNode, prevNode!);
                internalNode.SetKey(childIndex - 1, newKey);
                return new Result.Success();
            }

            if (hasNextSibling && HasMoreThanMinimumChildren(nextNode!))
            {
                var newKey = BorrowFromNext(childNode, nextNode!);
                internalNode.SetKey(childIndex, newKey);
                return new Result.Success();
            }

            if (hasPrevSibling)
            {
                Merge(prevNode!, childNode);
                InternalDeleteNoUnderflow(internalNode, childIndex);
                pager.Free(childNode.PageId.Index);
            }
            else
            {
                Merge(childNode, nextNode!);
                InternalDeleteNoUnderflow(internalNode, childIndex + 1);
                pager.Free(nextNode!.PageId.Index);
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

        private IndexKey BorrowFromPrev(Node node, Node prevNode)
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
        private IndexKey BorrowFromNext(Node node, Node nextNode)
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
                public IndexKey Key { get; }

                public KeyNotFound(IndexKey key)
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
