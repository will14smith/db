using System;
using SimpleDatabase.Execution.Tables;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;
using SimpleDatabase.Storage.Tree;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Execution.Values
{
    public class IndexCursor : ICursor, IInsertTarget, IDeleteTarget
    {
        private readonly Option<Cursor> _cursor;
        private readonly IPager _pager;
        private readonly IRowSerializer _rowSerializer;

        public Table Table { get; }
        public Index Index { get; }
        public bool Writable { get; }

        private PageSource Source => new PageSource.Index(Table.Name, Index.Name);
        private ISourcePager SourcePager => new SourcePager(_pager, Source);

        public IndexCursor(IPager pager, IRowSerializer rowSerializer, Table table, Index index, bool writable)
        {
            _pager = pager;
            _rowSerializer = rowSerializer;

            Table = table;
            Index = index;
            Writable = writable;
        }
        public IndexCursor(Cursor cursor, IndexCursor indexCursor)
            : this(indexCursor._pager, indexCursor._rowSerializer, indexCursor.Table, indexCursor.Index, indexCursor.Writable)
        {
            _cursor = Option.Some(cursor);
        }

        public bool EndOfTable => _cursor.Value.EndOfTable;

        public ICursor First()
        {
            var searcher = new TreeTraverser(SourcePager, _rowSerializer, Index);
            var cursor = searcher.StartCursor();

            return new IndexCursor(cursor, this);
        }

        public ICursor Next()
        {
            var searcher = new TreeTraverser(SourcePager, _rowSerializer, Index);
            var cursor = searcher.AdvanceCursor(_cursor.Value);

            return new IndexCursor(cursor, this);
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
