using System;
using SimpleDatabase.Execution.Trees;
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
            var page = _pager.Get(_cursor.Value.Page);
            var leaf = LeafNode.Read(_rowSerializer, page);

            return leaf.GetCellColumn(_cursor.Value.CellNumber, index);
        }

        public InsertResult Insert(Row row)
        {
            var inserter = new TreeInserter(SourcePager, _rowSerializer, Index);
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

            var nextCursor = (IndexCursor)Next();
            var nextKey = nextCursor.EndOfTable ? Option.None<int>() : Option.Some(GetKey(nextCursor._cursor.Value));

            var deleter = new TreeDeleter(SourcePager, _rowSerializer, Index);
            var deleteResult = deleter.Delete(key);

            switch (deleteResult)
            {
                case TreeDeleteResult.Success _:
                    {
                        var newCursor = nextKey.Map(FindKey, () => new Cursor(new PageId(Source, -1), -1, true));
                        
                        return new DeleteResult.Success(new IndexCursor(newCursor, this));
                    }

                case TreeDeleteResult.KeyNotFound _:
                    throw new NotImplementedException("TODO error handling!");

                default:
                    throw new NotImplementedException($"Unsupported type: {deleteResult.GetType().Name}");
            }
        }

        private Cursor FindKey(int key)
        {
            var searcher = new TreeSearcher(SourcePager, new TreeKeySearcher(key), _rowSerializer);

            return searcher.FindCursor(Index.RootPage);
        }

        private int GetKey(Cursor cursor)
        {
            var page = _pager.Get(cursor.Page);
            var leaf = LeafNode.Read(_rowSerializer, page);

            return leaf.GetCellKey(cursor.CellNumber);
        }
    }
}
