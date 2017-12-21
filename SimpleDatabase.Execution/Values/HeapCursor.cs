using System;
using SimpleDatabase.Execution.Trees;
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

        //private PageSource Source => new PageSource.Heap(Table.Name);
        //private ISourcePager SourcePager => new SourcePager(_pager, Source);

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
            throw new NotImplementedException();
        }

        public ICursor Next()
        {
            throw new NotImplementedException();
        }

        public int Key()
        {
            throw new NotImplementedException();
        }

        public ColumnValue Column(int index)
        {
            throw new NotImplementedException();
        }

        public InsertResult Insert(Row row)
        {
            throw new NotImplementedException();
        }

        public DeleteResult Delete()
        {
            throw new NotImplementedException();
        }
    }
}
