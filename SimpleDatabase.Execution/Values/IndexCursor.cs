using System;
using SimpleDatabase.Execution.Tables;
using SimpleDatabase.Execution.Transactions;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Execution.Values
{
    public class IndexCursor : ICursor, IInsertTarget, IDeleteTarget
    {
        private readonly Option<Cursor> _cursor;
        private readonly IPager _pager;
        private readonly ITransactionManager _txm;

        private readonly TreeTraverser _treeTraverser;

        public Table Table { get; }
        public Index Index { get; }
        public bool Writable { get; }

        public IndexCursor(IPager pager, ITransactionManager txm, Table table, Index index, bool writable)
        {
            _pager = pager;
            _txm = txm;

            _treeTraverser = new TreeTraverser(pager, txm, table, index);

            Table = table;
            Index = index;
            Writable = writable;
        }
        public IndexCursor(Cursor cursor, IndexCursor indexCursor)
            : this(indexCursor._pager, indexCursor._txm, indexCursor.Table, indexCursor.Index, indexCursor.Writable)
        {
            _cursor = Option.Some(cursor);
        }

        public bool EndOfTable => _cursor.Value.EndOfTable;

        public ICursor First()
        {
            var cursor = _treeTraverser.StartCursor();

            return new IndexCursor(cursor, this);
        }

        public ICursor Next()
        {
            var cursor = _treeTraverser.AdvanceCursor(_cursor.Value);

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
