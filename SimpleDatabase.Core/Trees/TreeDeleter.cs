using SimpleDatabase.Core.Paging;
using System;

namespace SimpleDatabase.Core.Trees
{
    public class TreeDeleter
    {
        private readonly IPager _pager;

        public TreeDeleter(IPager pager)
        {
            _pager = pager;
        }

        public DeleteResult Delete(int rootPageNumber, int key)
        {
            var page = _pager.Get(rootPageNumber);
            var node = Node.Read(page);

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
                    return new DeleteResult.Success(key);

                case Result.KeyNotFound keyNotFound:
                    return new DeleteResult.KeyNotFound(keyNotFound.Key);

                default:
                    throw new ArgumentOutOfRangeException($"Unhandled result: {result}");
            }
        }

        private Result LeafDelete(LeafNode node, int key)
        {
            var result = LeafDeleteNoUnderflow(node, key);
            if (result is Result.KeyNotFound)
            {
                return result;
            }

            var leafIsValid = node.IsRoot || node.CellCount > NodeLayout.LeafNodeMinCells;
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
            var childNode = Node.Read(childPage);

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

        private Result ResolveUnderflow(InternalNode internalNode, int childIndex, Node childNode)
        {
            var hasPrevSibling = childIndex > 0;
            var hasNextSibling = childIndex < internalNode.KeyCount;

            if (!hasPrevSibling && !hasNextSibling)
            {
                throw new InvalidOperationException("Cannot have a node with no siblings unless we are root (in which case this shouldn't have been called)");
            }

            var prevNode = hasPrevSibling ? Node.Read(_pager.Get(internalNode.GetChild(childIndex - 1))) : null;
            var nextNode = hasNextSibling ? Node.Read(_pager.Get(internalNode.GetChild(childIndex + 1))) : null;

            if (hasPrevSibling && HasMoreThanMinimumChildren(prevNode))
            {
                // move max(prevNode) -> start(childNode)
                // update key-1 in internalNode to new max(prevNode)
                throw new NotImplementedException("borrow from prev");
            }

            if (hasNextSibling && HasMoreThanMinimumChildren(nextNode))
            {
                // move min(nextNode) -> end(childNode),
                // update key in internalNode to new max(childNode)
                throw new NotImplementedException("borrow from next");
            }
            
            if (hasPrevSibling)
            {
                // merge prev & child
                throw new NotImplementedException("merge with prev");
            }
            else
            {
                // merge child & next   
                throw new NotImplementedException("merge with next");
            }

            if (internalNode.KeyCount <= NodeLayout.InternalNodeMinCells)
            {
                return new Result.NodeUnderflow(childNode);
            }

            return new Result.Success();
        }

        private bool HasMoreThanMinimumChildren(Node node)
        {
            switch (node)
            {
                case LeafNode leafNode:
                    return leafNode.CellCount > NodeLayout.LeafNodeMinCells;
                    break;
                case InternalNode internalNode:
                    return internalNode.KeyCount > NodeLayout.InternalNodeMinCells;
                    break;
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
