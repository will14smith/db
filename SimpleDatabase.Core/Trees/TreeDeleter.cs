﻿using SimpleDatabase.Core.Paging;
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

            var leafIsValid = node.IsRoot || node.CellCount >= NodeLayout.LeafNodeMinCells;
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
                    var leafNextNode = (LeafNode) nextNode;
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

        private bool HasMoreThanMinimumChildren(Node node)
        {
            switch (node)
            {
                case LeafNode leafNode:
                    return leafNode.CellCount > NodeLayout.LeafNodeMinCells;
                case InternalNode internalNode:
                    return internalNode.KeyCount > NodeLayout.InternalNodeMinCells;
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
