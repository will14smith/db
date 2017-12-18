using System;
using SimpleDatabase.Execution.Trees;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;
using SimpleDatabase.Storage.Tree;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Execution.Values
{
    public class TableCursor : ICursor, IInsertTarget, IDeleteTarget
    {
        private readonly Option<Cursor> _cursor;
        private readonly IPager _pager;
        private readonly IRowSerializer _rowSerializer;

        public Table Table { get; }
        public bool Writable { get; }

        public TableCursor(IPager pager, IRowSerializer rowSerializer, Table table, bool writable)
        {
            _pager = pager;
            _rowSerializer = rowSerializer;

            Table = table;
            Writable = writable;
        }
        public TableCursor(Cursor cursor, TableCursor tableCursor)
            : this(tableCursor._pager, tableCursor._rowSerializer, tableCursor.Table, tableCursor.Writable)
        {
            _cursor = Option.Some(cursor);
        }

        public bool EndOfTable => _cursor.Value.EndOfTable;

        public ICursor First()
        {
            var searcher = new TreeTraverser(_pager, _rowSerializer, Table);
            var cursor = searcher.StartCursor();

            return new TableCursor(cursor, this);
        }

        public ICursor Next()
        {
            var searcher = new TreeTraverser(_pager, _rowSerializer, Table);
            var cursor = searcher.AdvanceCursor(_cursor.Value);

            return new TableCursor(cursor, this);
        }

        public int Key()
        {
            throw new NotImplementedException();
        }

        public ColumnValue Column(int index)
        {
            var page = _pager.Get(_cursor.Value.PageNumber);
            var leaf = LeafNode.Read(_rowSerializer, page);

            return leaf.GetCellColumn(_cursor.Value.CellNumber, index);
        }

        public InsertResult Insert(Row row)
        {
            var inserter = new TreeInserter(_pager, _rowSerializer, Table);
            var insertResult = inserter.Insert(row.GetKey(), row);
            switch (insertResult)
            {
                case TreeInsertResult.Success _:
                    return new InsertResult.Success();

                case TreeInsertResult.DuplicateKey _:
                    throw new NotImplementedException("TODO error handling!");

                default:
                    throw new NotImplementedException($"Unsupported type: {insertResult.GetType().Name}");
            }
        }

        public DeleteResult Delete()
        {
            var table = Table;
            var rowSerializer = _rowSerializer;

            // TODO could this be optimised to not retraverse the tree in TreeDeleter?
            var key = GetKey(_cursor.Value);

            var nextCursor = (TableCursor)Next();
            var nextKey = nextCursor.EndOfTable ? Option.None<int>() : Option.Some(GetKey(nextCursor._cursor.Value));

            var deleter = new TreeDeleter(_pager, rowSerializer, table);
            var deleteResult = deleter.Delete(key);

            switch (deleteResult)
            {
                case TreeDeleteResult.Success _:
                    {
                        var newCursor = nextKey.Map(FindKey, () => new Cursor(-1, -1, true));
                        
                        return new DeleteResult.Success(new TableCursor(newCursor, this));
                    }

                case TreeDeleteResult.KeyNotFound _:
                    throw new NotImplementedException("TODO error handling!");

                default:
                    throw new NotImplementedException($"Unsupported type: {deleteResult.GetType().Name}");
            }
        }

        private Cursor FindKey(int key)
        {
            var searcher = new TreeSearcher(_pager, new TreeKeySearcher(key), _rowSerializer);

            return searcher.FindCursor(Table.RootPageId);
        }

        private int GetKey(Cursor cursor)
        {
            var page = _pager.Get(cursor.PageNumber);
            var leaf = LeafNode.Read(_rowSerializer, page);

            return leaf.GetCellKey(cursor.CellNumber);
        }
    }
}
