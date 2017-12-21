using System;
using SimpleDatabase.Execution.Tables;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Execution.Values
{
    public class HeapCursor : ICursor, IInsertTarget, IDeleteTarget
    {
        private readonly Option<Cursor> _cursor;
        private readonly IPager _pager;
        private readonly IRowSerializer _rowSerializer;

        public Table Table { get; }
        public bool Writable { get; }

        private PageSource Source => new PageSource.Heap(Table.Name);
        private ISourcePager SourcePager => new SourcePager(_pager, Source);

        public HeapCursor(IPager pager, IRowSerializer rowSerializer, Table table, bool writable)
        {
            _pager = pager;
            _rowSerializer = rowSerializer;

            Table = table;
            Writable = writable;
        }
        public HeapCursor(Cursor cursor, HeapCursor heapCursor)
            : this(heapCursor._pager, heapCursor._rowSerializer, heapCursor.Table, heapCursor.Writable)
        {
            _cursor = Option.Some(cursor);
        }

        public bool EndOfTable => _cursor.Value.EndOfTable;

        public ICursor First()
        {
            var traverser = new HeapTraverser(SourcePager, Table);
            var cursor = traverser.StartCursor();

            return new HeapCursor(cursor, this);
        }

        public ICursor Next()
        {
            var traverser = new HeapTraverser(SourcePager, Table);
            var cursor = traverser.AdvanceCursor(_cursor.Value);

            return new HeapCursor(cursor, this);
        }

        public int Key()
        {
            throw new NotImplementedException();
        }

        public ColumnValue Column(int index)
        {
            throw new NotImplementedException();
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
            var result = deleter.Delete(Key(), _cursor.Value);

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
