using System;
using SimpleDatabase.Core.Paging;

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
            var result = LeafDeleteNoMerge(node, key);
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

        private Result LeafDeleteNoMerge(LeafNode node, int key)
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

            // if !hasPrev && !hasNext => panic!

            // if hasPrev && prevNode has spares => move max(prevNode) -> start(childNode), update key in internalNode
            // if hasNext && nextNode has spares => move min(prevNode) -> end(childNode),   update key in internalNode

            // if hasPrev => merge prev & child
            // if hasNext => merge child & next

            throw new NotImplementedException("Implement ResolveUnderflow");
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
