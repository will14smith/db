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

            new TreeInserter(_pager).LeafNodeInsert(cursor, row.Id, row);

            return new InsertResult.Success(row.Id);
        }

        public SelectResult Select(SelectStatement statement)
        {
            var traverser = new TreeTraverser(this, _pager);
            
            var cursor = traverser.StartCursor();

            var rows = new List<Row>();
            while (!cursor.EndOfTable)
            {
                var page = _pager.Get(cursor.PageNumber);
                var leaf = LeafNode.Read(page);

                var row = Row.Deserialize(page.Data, leaf.GetCellValueOffset(cursor.CellNumber));
                cursor = traverser.AdvanceCursor(cursor);

                rows.Add(row);
            }

            return new SelectResult.Success(rows);
        }

        private Cursor FindCursor(int key)
        {
            return new TreeKeySearcher(_pager, key).FindCursor(RootPageNumber);
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
