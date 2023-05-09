using System;
using SimpleDatabase.Execution.Trees;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;
using SimpleDatabase.Storage.Tree;

namespace SimpleDatabase.Execution.Tables
{
    public class TreeInserter
    {
        private readonly TableManager _tableManager;
        private readonly TableIndex _index;
        private readonly IIndexSerializer _serializer;

        public TreeInserter(TableManager tableManager, TableIndex index)
        {
            _tableManager = tableManager;
            _index = index;
            _serializer = index.CreateSerializer();
        }

        public InsertResult Insert(IndexKey key, IndexData data)
        {
            var page = _tableManager.Pager.Get(_tableManager.GetIndexRootPageId(_index));
            var node = Node.Read(page, _serializer);

            Result result;
            switch (node)
            {
                case LeafNode leafNode:
                    result = LeafInsert(leafNode, key, data);
                    break;
                case InternalNode internalNode:
                    result = InternalInsert(internalNode, key, data);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (result)
            {
                case Result.Success _:
                    return new InsertResult.Success();
                case Result.DuplicateKey _:
                    return new InsertResult.DuplicateKey();

                case Result.WasSplit split:
                    SplitRoot(split);
                    return new InsertResult.Success();

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SplitRoot(Result.WasSplit split)
        {
            var rootPageId = _tableManager.GetIndexRootPageId(_index);
            
            if (rootPageId.Index != split.Left)
            {
                throw new InvalidOperationException("Uhm...?");
            }

            var pager = _tableManager.Pager;
            
            var leftPage = pager.Allocate();
            var rightPage = pager.Get(split.Right);
            var rootPage = pager.Get(split.Left);

            Array.Copy(rootPage.Data, leftPage.Data, PageLayout.PageSize);
            var leftNode = Node.Read(leftPage, _serializer);
            leftNode.IsRoot = false;

            var rootNode = InternalNode.New(rootPage, _serializer);
            rootNode.IsRoot = true;

            rootNode.KeyCount = 1;
            rootNode.SetCell(0, leftPage.Id.Index, split.Key);
            rootNode.SetChild(1, rightPage.Id.Index);
        }

        private Result LeafInsert(LeafNode node, IndexKey key, IndexData data)
        {
            var cellIndex = new TreeKeySearcher(key).FindCell(node);

            if (node.CellCount < node.Layout.LeafNodeMaxCells)
            {
                return LeafInsertNonFull(node, key, data, cellIndex);
            }

            var newPage = _tableManager.Pager.Allocate();
            var newNode = LeafNode.New(newPage, _serializer);

            newNode.NextLeaf = node.NextLeaf;
            node.NextLeaf = newPage.Id.Index;

            var threshold = newNode.Layout.LeafNodeLeftSplitCount;
            for (var j = 0; j < newNode.Layout.LeafNodeRightSplitCount; ++j)
            {
                newNode.CopyCell(node, threshold + j, j);
            }

            newNode.CellCount = newNode.Layout.LeafNodeRightSplitCount;
            node.CellCount = node.Layout.LeafNodeLeftSplitCount;

            if (cellIndex < threshold)
            {
                LeafInsertNonFull(node, key, data, cellIndex);
            }
            else
            {
                LeafInsertNonFull(newNode, key, data, cellIndex - threshold);
            }

            return new Result.WasSplit(
                node.GetMaxKey(),
                node.PageId.Index,
                newNode.PageId.Index
            );
        }

        private Result LeafInsertNonFull(LeafNode node, IndexKey key, IndexData data, int cellIndex)
        {
            if (node.CellCount >= node.Layout.LeafNodeMaxCells)
            {
                throw new InvalidOperationException("Leaf would overflow");
            }
            if (cellIndex >= node.Layout.LeafNodeMaxCells)
            {
                throw new InvalidOperationException("CellIndex out of bounds");
            }
            if (cellIndex > node.CellCount)
            {
                throw new InvalidOperationException("CellIndex would leave gaps");
            }

            if (cellIndex < node.CellCount && node.GetCellKey(cellIndex) == key)
            {
                return new Result.DuplicateKey();
            }

            if (cellIndex < node.CellCount)
            {
                for (var i = node.CellCount; i > cellIndex; i--)
                {
                    node.CopyCell(node, i - 1, i);
                }
            }

            node.CellCount += 1;
            node.SetCell(cellIndex, key, data);

            return new Result.Success();
        }

        private Result InternalInsert(InternalNode node, IndexKey key, IndexData data)
        {
            if (node.KeyCount < node.Layout.InternalNodeMaxCells)
            {
                return InnerInsertNonFull(node, key, data);
            }

            // this split is pre-emptive as there might be space in the children
            // all the B+ tree invariants still hold though

            var newPage = _tableManager.Pager.Allocate();
            var newNode = InternalNode.New(newPage, _serializer);

            var threshold = newNode.Layout.InternalNodeLeftSplitCount;
            var splitKey = node.GetKey(threshold - 1);

            newNode.KeyCount = newNode.Layout.InternalNodeRightSplitCount;
            for (var j = 0; j < newNode.Layout.InternalNodeRightSplitCount; ++j)
            {
                newNode.CopyCell(node, threshold + j, j);
            }

            newNode.SetChild(newNode.Layout.InternalNodeRightSplitCount, node.RightChild);
            var rightChild = node.GetChild(threshold - 1);
            node.KeyCount = threshold - 1;
            node.SetChild(threshold - 1, rightChild);

            InnerInsertNonFull(key < splitKey ? node : newNode, key, data);

            return new Result.WasSplit(splitKey, node.PageId.Index, newNode.PageId.Index);
        }

        private Result InnerInsertNonFull(InternalNode node, IndexKey key, IndexData data)
        {
            var cellIndex = new TreeKeySearcher(key).FindCell(node);

            var childPageNumber = node.GetChild(cellIndex);
            var childPage = _tableManager.Pager.Get(childPageNumber);
            var childNode = Node.Read(childPage, _serializer);

            Result result;
            switch (childNode)
            {
                case LeafNode leafNode:
                    result = LeafInsert(leafNode, key, data);
                    break;
                case InternalNode internalNode:
                    result = InternalInsert(internalNode, key, data);
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

            public class DuplicateKey : Result { }

            public class WasSplit : Result
            {
                public WasSplit(IndexKey key, int left, int right)
                {
                    Key = key;
                    Left = left;
                    Right = right;
                }

                public IndexKey Key { get; }
                public int Left { get; }
                public int Right { get; }
            }
        }
    }
}
