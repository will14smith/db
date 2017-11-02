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
                LeafNode.New(rootPage);
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


            if (leaf.CellCount >= NodeLayout.LeafNodeMaxCells)
            {
                return new InsertResult.TableFull();
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
                throw new NotImplementedException("Split the leaf node");
            }

            leaf.InsertCell(cursor.CellNumber, key, value);
            _pager.Flush(cursor.PageNumber);
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
