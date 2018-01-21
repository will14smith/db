﻿using System;
using SimpleDatabase.Execution.Tables;
using SimpleDatabase.Execution.Transactions;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Heap;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Execution.Values
{
    public class HeapCursor : ICursor, IInsertTarget, IDeleteTarget
    {
        private readonly Option<Cursor> _cursor;
        private readonly IPager _pager;
        private readonly ITransactionManager _txm;

        private readonly ISourcePager _sourcePager;
        private readonly IHeapSerializer _heapSerializer;

        public Table Table { get; }
        public bool Writable { get; }

        public HeapCursor(IPager pager, ITransactionManager txm, Table table, bool writable)
        {
            _pager = pager;
            _txm = txm;

            _sourcePager = new SourcePager(_pager, new PageSource.Heap(table.Name));
            _heapSerializer = new HeapSerializer(table.Columns, new ColumnTypeSerializerFactory());

            Table = table;
            Writable = writable;
        }
        public HeapCursor(Cursor cursor, HeapCursor heapCursor)
            : this(heapCursor._pager, heapCursor._txm, heapCursor.Table, heapCursor.Writable)
        {
            _cursor = Option.Some(cursor);
        }

        public bool EndOfTable => _cursor.Value.EndOfTable;

        public ICursor First()
        {
            var traverser = new HeapTraverser(_sourcePager, _txm, _heapSerializer);
            var cursor = traverser.StartCursor();

            return new HeapCursor(cursor, this);
        }

        public ICursor Next()
        {
            var traverser = new HeapTraverser(_sourcePager, _txm, _heapSerializer);
            var cursor = traverser.AdvanceCursor(_cursor.Value);

            return new HeapCursor(cursor, this);
        }

        public int Key()
        {
            throw new NotImplementedException();
        }

        public ColumnValue Column(int index)
        {
            var cursor = _cursor.Value;

            var page = HeapPage.Read(_sourcePager.Get(cursor.Page.Index));
            var cell = page.GetItem(cursor.CellNumber);

            return _heapSerializer.ReadColumn(cell, index);
        }

        public InsertTargetResult Insert(Row row)
        {
            var inserter = new TableInserter(_pager, Table);
            var result = inserter.Insert(row);

            switch (result)
            {
                case InsertResult.Success _:
                    return new InsertTargetResult.Success();

                case InsertResult.DuplicateKey _:
                    throw new NotImplementedException("TODO error handling!");

                default:
                    throw new NotImplementedException($"Unsupported type: {result.GetType().Name}");
            }
        }

        public DeleteTargetResult Delete()
        {
            var deleter = new TableDeleter(_pager, Table);
            var result = deleter.Delete(_cursor.Value);

            switch (result)
            {
                case DeleteResult.Success _:
                    throw new NotImplementedException("Figure out next key");
                    
                case DeleteResult.KeyNotFound _:
                    throw new NotImplementedException("TODO error handling!");

                default:
                    throw new NotImplementedException($"Unsupported type: {result.GetType().Name}");
            }
        }
    }
}
