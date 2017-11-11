using System;
using SimpleDatabase.Core.Paging;

namespace SimpleDatabase.Core.Trees
{
    public class TreeInserter
    {
        private readonly IPager _pager;

        public TreeInserter(IPager pager)
        {
            _pager = pager;
        }

        public InsertResult Insert(int rootPageNumber, int key, Row row)
        {
            var page = _pager.Get(rootPageNumber);
            var node = Node.Read(page);

            Result result;
            switch (node)
            {
                case LeafNode leafNode:
                    result = LeafInsert(leafNode, key, row);
                    break;
                case InternalNode internalNode:
                    result = InternalInsert(internalNode, key, row);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (result)
            {
                case Result.Success _:
                    return new InsertResult.Success(key);
                case Result.DuplicateKey r:
                    return new InsertResult.DuplicateKey(r.Key);

                case Result.WasSplit split:
                    SplitRoot(rootPageNumber, split);
                    return new InsertResult.Success(key);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SplitRoot(int rootPageNumber, Result.WasSplit split)
        {
            if (rootPageNumber != split.Left)
            {
                throw new InvalidOperationException("Uhm...?");
            }

            var leftPage = _pager.GetUnusedPage();
            var rightPage = _pager.Get(split.Right);
            var rootPage = _pager.Get(split.Left);

            Array.Copy(rootPage.Data, leftPage.Data, Pager.PageSize);
            var leftNode = Node.Read(leftPage);
            leftNode.IsRoot = false;

            var rootNode = InternalNode.New(rootPage);
            rootNode.IsRoot = true;

            rootNode.KeyCount = 1;
            rootNode.SetCell(0, leftPage.Number, split.Key);
            rootNode.SetChild(1, rightPage.Number);
        }

        private Result LeafInsert(LeafNode node, int key, Row row)
        {
            var cellIndex = new TreeKeySearcher(key).FindCell(node);

            if (node.CellCount < NodeLayout.LeafNodeMaxCells)
            {
                return LeafInsertNonFull(node, key, row, cellIndex);
            }

            var newPage = _pager.GetUnusedPage();
            var newNode = LeafNode.New(newPage);

            newNode.NextLeaf = node.NextLeaf;
            node.NextLeaf = newPage.Number;

            var threshold = NodeLayout.LeafNodeLeftSplitCount;
            for (var j = 0; j < NodeLayout.LeafNodeRightSplitCount; ++j)
            {
                newNode.CopyCell(node, threshold + j, j);
            }

            newNode.CellCount = NodeLayout.LeafNodeRightSplitCount;
            node.CellCount = NodeLayout.LeafNodeLeftSplitCount;

            if (cellIndex < threshold)
            {
                LeafInsertNonFull(node, key, row, cellIndex);
            }
            else
            {
                LeafInsertNonFull(newNode, key, row, cellIndex - threshold);
            }

            return new Result.WasSplit(
                node.GetMaxKey(),
                node.PageNumber,
                newNode.PageNumber
            );
        }

        private Result LeafInsertNonFull(LeafNode node, int key, Row row, int cellIndex)
        {
            if (node.CellCount >= NodeLayout.LeafNodeMaxCells)
            {
                throw new InvalidOperationException("Leaf would overflow");
            }
            if (cellIndex >= NodeLayout.LeafNodeMaxCells)
            {
                throw new InvalidOperationException("CellIndex out of bounds");
            }
            if (cellIndex > node.CellCount)
            {
                throw new InvalidOperationException("CellIndex would leave gaps");
            }

            if (cellIndex < node.CellCount && node.GetCellKey(cellIndex) == key)
            {
                return new Result.DuplicateKey(key);
            }

            if (cellIndex < node.CellCount)
            {
                for (var i = node.CellCount; i > cellIndex; i--)
                {
                    node.CopyCell(node, i - 1, i);
                }
            }

            node.CellCount += 1;
            node.SetCell(cellIndex, key, row);

            return new Result.Success();
        }

        private Result InternalInsert(InternalNode node, int key, Row row)
        {
            if (node.KeyCount < NodeLayout.InternalNodeMaxCells)
            {
                return InnerInsertNonFull(node, key, row);
            }

            // this split is pre-emptive as there might be space in the children
            // all the B+ tree invariants still hold though

            var newPage = _pager.GetUnusedPage();
            var newNode = InternalNode.New(newPage);

            var threshold = NodeLayout.InternalNodeLeftSplitCount;
            var splitKey = node.GetKey(threshold - 1);

            newNode.KeyCount = NodeLayout.InternalNodeRightSplitCount;
            for (var j = 0; j < NodeLayout.InternalNodeRightSplitCount; ++j)
            {
                newNode.CopyCell(node, threshold + j, j);
            }

            newNode.SetChild(NodeLayout.InternalNodeRightSplitCount, node.RightChild);
            var rightChild = node.GetChild(threshold - 1);
            node.KeyCount = threshold - 1;
            node.SetChild(threshold - 1, rightChild);

            InnerInsertNonFull(key < splitKey ? node : newNode, key, row);
            
            return new Result.WasSplit(splitKey, node.PageNumber, newNode.PageNumber);
        }

        private Result InnerInsertNonFull(InternalNode node, int key, Row row)
        {
            var cellIndex = new TreeKeySearcher(key).FindCell(node);

            var childPageNumber = node.GetChild(cellIndex);
            var childPage = _pager.Get(childPageNumber);
            var childNode = Node.Read(childPage);

            Result result;
            switch (childNode)
            {
                case LeafNode leafNode:
                    result = LeafInsert(leafNode, key, row);
                    break;
                case InternalNode internalNode:
                    result = InternalInsert(internalNode, key, row);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (result)
            {
                case Result.WasSplit split:
                    if (cellIndex == node.KeyCount)
                    {
                        node.KeyCount += 1;
                        node.SetCell(cellIndex, split.Left, split.Key);
                        node.SetChild(cellIndex + 1, split.Right);
                    }
                    else
                    {
                        node.KeyCount += 1;
                        for (var i = node.KeyCount; i > cellIndex; i--)
                        {
                            node.CopyCell(node, i - 1, i);
                        }

                        node.SetCell(cellIndex, split.Left, split.Key);
                        node.SetChild(cellIndex + 1, split.Right);
                    }

                    return new Result.Success();

                default:
                    return result;
            }
        }

        private abstract class Result
        {
            public class Success : Result { }

            public class DuplicateKey : Result
            {
                public DuplicateKey(int key)
                {
                    Key = key;
                }

                public int Key { get; }
            }

            public class WasSplit : Result
            {
                public WasSplit(int key, int left, int right)
                {
                    Key = key;
                    Left = left;
                    Right = right;
                }

                public int Key { get; }
                public int Left { get; }
                public int Right { get; }
            }
        }
    }
}
