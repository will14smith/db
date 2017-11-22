﻿using System;
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

            return new TreeInserter(_pager).Insert(RootPageNumber, row.Id, row);
        }

        public SelectResult Select(SelectStatement statement)
        {
            var traverser = new TreeTraverser(_pager);
            var cursor = traverser.StartCursor(RootPageNumber);

            var rows = new List<Row>();
            while (!cursor.EndOfTable)
            {
                var page = _pager.Get(cursor.PageNumber);
                var leaf = LeafNode.Read(page);

                var row = leaf.GetCellValue(cursor.CellNumber);
                cursor = traverser.AdvanceCursor(cursor);

                rows.Add(row);
            }

            return new SelectResult.Success(rows);
        }

        public DeleteResult Delete(DeleteStatement delete)
        {
            return new TreeDeleter(_pager).Delete(RootPageNumber, delete.Key);
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
            public int Key { get; }

            public Success(int key)
            {
                Key = key;
            }
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

    public abstract class DeleteResult
    {
        public class Success : DeleteResult
        {
            public int Key { get; }

            public Success(int key)
            {
                Key = key;
            }
        }

        public class KeyNotFound : DeleteResult
        {
            public int Key { get; }

            public KeyNotFound(int key)
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
