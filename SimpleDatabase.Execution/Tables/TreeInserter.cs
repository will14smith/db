﻿using System;
using SimpleDatabase.Execution.Trees;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;
using SimpleDatabase.Storage.Tree;

namespace SimpleDatabase.Execution.Tables
{
    public class TreeInserter
    {
        private readonly ISourcePager _pager;

        private readonly RowSerializer _keySerializer;
        private readonly RowSerializer _dataSerializer;

        private readonly Index _index;

        public TreeInserter(ISourcePager pager, Index index)
        {
            _pager = pager;

            var (keyColumns, dataColumns) = index.GetPersistenceColumns();

            _keySerializer = new RowSerializer(keyColumns, new ColumnTypeSerializerFactory());
            _dataSerializer = new RowSerializer(dataColumns, new ColumnTypeSerializerFactory());

            _index = index;
        }

        public InsertResult Insert(int key, Row row)
        {
            var page = _pager.Get(_index.RootPage);
            var node = Node.Read(page, _keySerializer, _dataSerializer);

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
                    SplitRoot(split);
                    return new InsertResult.Success(key);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SplitRoot(Result.WasSplit split)
        {
            if (_index.RootPage != split.Left)
            {
                throw new InvalidOperationException("Uhm...?");
            }

            var leftPage = _pager.Allocate();
            var rightPage = _pager.Get(split.Right);
            var rootPage = _pager.Get(split.Left);

            Array.Copy(rootPage.Data, leftPage.Data, PageLayout.PageSize);
            var leftNode = Node.Read(leftPage, _keySerializer, _dataSerializer);
            leftNode.IsRoot = false;

            var rootNode = InternalNode.New(rootPage, _keySerializer, _dataSerializer);
            rootNode.IsRoot = true;

            rootNode.KeyCount = 1;
            rootNode.SetCell(0, leftPage.Id.Index, split.Key);
            rootNode.SetChild(1, rightPage.Id.Index);
        }

        private Result LeafInsert(LeafNode node, int key, Row row)
        {
            var cellIndex = new TreeKeySearcher(key).FindCell(node);

            if (node.CellCount < node.Layout.LeafNodeMaxCells)
            {
                return LeafInsertNonFull(node, key, row, cellIndex);
            }

            var newPage = _pager.Allocate();
            var newNode = LeafNode.New(newPage, _keySerializer, _dataSerializer);

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
                LeafInsertNonFull(node, key, row, cellIndex);
            }
            else
            {
                LeafInsertNonFull(newNode, key, row, cellIndex - threshold);
            }

            return new Result.WasSplit(
                node.GetMaxKey(),
                node.PageId.Index,
                newNode.PageId.Index
            );
        }

        private Result LeafInsertNonFull(LeafNode node, int key, Row row, int cellIndex)
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
            if (node.KeyCount < node.Layout.InternalNodeMaxCells)
            {
                return InnerInsertNonFull(node, key, row);
            }

            // this split is pre-emptive as there might be space in the children
            // all the B+ tree invariants still hold though

            var newPage = _pager.Allocate();
            var newNode = InternalNode.New(newPage, _keySerializer, _dataSerializer);

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

            InnerInsertNonFull(key < splitKey ? node : newNode, key, row);

            return new Result.WasSplit(splitKey, node.PageId.Index, newNode.PageId.Index);
        }

        private Result InnerInsertNonFull(InternalNode node, int key, Row row)
        {
            var cellIndex = new TreeKeySearcher(key).FindCell(node);

            var childPageNumber = node.GetChild(cellIndex);
            var childPage = _pager.Get(childPageNumber);
            var childNode = Node.Read(childPage, _keySerializer, _dataSerializer);

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
