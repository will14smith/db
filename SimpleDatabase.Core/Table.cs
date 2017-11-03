using System;
using System.Collections.Generic;
using SimpleDatabase.Core.Paging;
using SimpleDatabase.Core.Trees;

namespace SimpleDatabase.Core
{
    public class Table : IDisposable
    {
        private readonly IPager _pager;

        public int RootPageNumber { get; }

        public Table(IPager pager)
        {
            _pager = pager;

            RootPageNumber = 0;
            if (_pager.PageCount == 0)
            {
                var rootPage = _pager.Get(RootPageNumber);
                var node = LeafNode.New(rootPage);
                node.IsRoot = true;
                _pager.Flush(RootPageNumber);
            }
        }

        public InsertResult Insert(InsertStatement statement)
        {
            var row = statement.Row;

            var keyToInsert = row.Id;
            var cursor = FindCursor(keyToInsert);

            var page = _pager.Get(cursor.PageNumber);
            var leaf = LeafNode.Read(page);

            if (cursor.CellNumber < leaf.CellCount)
            {
                var keyAtIndex = leaf.GetCellKey(cursor.CellNumber);
                if (keyAtIndex == keyToInsert)
                {
                    return new InsertResult.DuplicateKey(keyToInsert);
                }
            }

            LeafNodeInsert(cursor, row.Id, row);

            return new InsertResult.Success(row.Id);
        }

        public SelectResult Select(SelectStatement statement)
        {
            var cursor = StartCursor();

            var rows = new List<Row>();
            while (!cursor.EndOfTable)
            {
                var page = _pager.Get(cursor.PageNumber);
                var leaf = LeafNode.Read(page);

                var row = Row.Deserialize(page.Data, leaf.GetCellValueOffset(cursor.CellNumber));
                cursor = AdvanceCursor(cursor);

                rows.Add(row);
            }

            return new SelectResult.Success(rows);
        }


        private Cursor StartCursor()
        {
            var page = _pager.Get(RootPageNumber);
            var leaf = LeafNode.Read(page);

            return new Cursor(
                this,
                RootPageNumber,
                0,
                leaf.CellCount == 0
            );
        }

        private Cursor FindCursor(int key)
        {
            var rootPage = _pager.Get(RootPageNumber);
            var rootNodeType = Node.GetType(rootPage);

            if (rootNodeType == NodeType.Leaf)
            {
                return LeafNodeFind(RootPageNumber, key);
            }
            else
            {
                throw new NotImplementedException("Searching an internal node");
            }
        }


        private Cursor AdvanceCursor(Cursor cursor)
        {
            var pageNumber = cursor.PageNumber;
            var page = _pager.Get(pageNumber);
            var leaf = LeafNode.Read(page);

            var cellNumber = cursor.CellNumber + 1;

            return new Cursor(
                this,
                pageNumber,
                cellNumber,
                cellNumber >= leaf.CellCount
            );
        }

        private Cursor LeafNodeFind(int pageNumber, int key)
        {
            var page = _pager.Get(pageNumber);
            var node = LeafNode.Read(page);

            var minIndex = 0;
            var onePastMaxIndex = node.CellCount;
            while (onePastMaxIndex != minIndex)
            {
                var index = (minIndex + onePastMaxIndex) / 2;
                var keyAtIndex = node.GetCellKey(index);
                if (key == keyAtIndex)
                {
                    minIndex = index;
                    break;
                }

                if (key < keyAtIndex)
                {
                    onePastMaxIndex = index;
                }
                else
                {
                    minIndex = index + 1;
                }
            }

            return new Cursor(
                this,
                pageNumber,
                minIndex,
                minIndex >= node.CellCount
            );
        }

        private void LeafNodeInsert(Cursor cursor, int key, Row value)
        {
            var page = _pager.Get(cursor.PageNumber);
            var leaf = LeafNode.Read(page);

            if (leaf.CellCount >= NodeLayout.LeafNodeMaxCells)
            {
                LeafNodeSplitAndInsert(cursor, key, value);
            }
            else
            {
                leaf.InsertCell(cursor.CellNumber, key, value);
                _pager.Flush(cursor.PageNumber);
            }
        }

        private void LeafNodeSplitAndInsert(Cursor cursor, int key, Row value)
        {
            /*
             * Create a new node and move half the cells over.
             * Insert the new value in one of the two nodes.
             * Update parent or create a new parent.
             */

            var oldPage = _pager.Get(cursor.PageNumber);
            var oldNode = LeafNode.Read(oldPage);
            var (newPage, newPageNum) = _pager.GetUnusedPage();
            var newNode = LeafNode.New(newPage);

            /*
             * All existing keys plus new key should should be divided
             * evenly between old (left) and new (right) nodes.
             * Starting from the right, move each key to correct position.
             */
            for (var i = NodeLayout.LeafNodeMaxCells; i >= 0; i--)
            {
                var destinationNode = i >= NodeLayout.LeafNodeLeftSplitCount ? newNode : oldNode;
                var indexWithinNode = i % NodeLayout.LeafNodeLeftSplitCount;

                if (i == cursor.CellNumber)
                {
                    destinationNode.SetCell(indexWithinNode, key, value);
                }
                else if (i > cursor.CellNumber)
                {
                    destinationNode.CopyCell(oldNode, i - 1, indexWithinNode);
                }
                else
                {
                    destinationNode.CopyCell(oldNode, i, indexWithinNode);
                }
            }

            // Update cell count on both leaf nodes
            oldNode.CellCount = NodeLayout.LeafNodeLeftSplitCount;
            newNode.CellCount = NodeLayout.LeafNodeRightSplitCount;

            if (oldNode.IsRoot)
            {
                CreateNewRoot(newPageNum);
            }
            else
            {
                throw new NotImplementedException("Update parent after split");
            }
        }

        private void CreateNewRoot(int rightChildPageNum)
        {
            /*
             * Handle splitting the root.
             * Old root copied to new page, becomes left child.
             * Address of right child passed in.
             * Re-initialize root page to contain the new root node.
             * New root node points to two children.
             */

            var rootPage = _pager.Get(RootPageNumber);
            var (leftChild, leftChildPageNum) = _pager.GetUnusedPage();

            // Left child has data copied from old root
            Array.Copy(rootPage.Data, leftChild.Data, Pager.PageSize);
            var leftNode = Node.Read(leftChild);
            leftNode.IsRoot = false;

            // Root node is a new internal node with one key and two children
            var rootNode = InternalNode.New(rootPage);
            rootNode.IsRoot = true;

            rootNode.KeyCount = 1;

            var leftChildMaxKey = leftNode.GetMaxKey();
            rootNode.SetCell(0, leftChildPageNum, leftChildMaxKey);
            rootNode.SetChild(1, rightChildPageNum);
        }

        public void Dispose()
        {
            _pager?.Dispose();
        }
    }

    public abstract class InsertResult
    {
        public class Success : InsertResult
        {
            public int RowNumber { get; }

            public Success(int rowNumber)
            {
                RowNumber = rowNumber;
            }
        }

        public class TableFull : InsertResult
        {
        }

        public class DuplicateKey : InsertResult
        {
            public int Key { get; }

            public DuplicateKey(int key)
            {
                Key = key;
            }
        }
    }

    public abstract class SelectResult
    {
        public class Success : SelectResult
        {
            public IReadOnlyCollection<Row> Rows { get; }

            public Success(IReadOnlyCollection<Row> rows)
            {
                Rows = rows;
            }
        }
    }
}
