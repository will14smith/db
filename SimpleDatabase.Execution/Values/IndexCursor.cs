using System;
using System.Linq;
using SimpleDatabase.Execution.Tables;
using SimpleDatabase.Execution.Transactions;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage;
using SimpleDatabase.Storage.Heap;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;
using SimpleDatabase.Storage.Tree;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Execution.Values
{
    public class IndexCursor : ICursor, IInsertTarget, IDeleteTarget
    {
        private readonly Option<Cursor> _cursor;
        private readonly TableManager _tableManager;
        private readonly ITransactionManager _txm;

        private readonly TreeTraverser _treeTraverser;
        private readonly IIndexSerializer _treeSerializer;

        public TableIndex Index { get; }
        public bool Writable { get; }

        public IndexCursor(TableManager tableManager, ITransactionManager txm, TableIndex index, bool writable)
        {
            _tableManager = tableManager;
            _txm = txm;

            _treeTraverser = new TreeTraverser(tableManager, txm, index);
            _treeSerializer = index.CreateSerializer();

            Index = index;
            Writable = writable;
        }
        public IndexCursor(Cursor cursor, IndexCursor indexCursor)
            : this(indexCursor._tableManager, indexCursor._txm, indexCursor.Index, indexCursor.Writable)
        {
            _cursor = Option.Some(cursor);
        }

        public bool EndOfTable => _cursor.HasValue && _cursor.Value!.EndOfTable;

        public ICursor First()
        {
            var cursor = _treeTraverser.StartCursor();

            return new IndexCursor(cursor, this);
        }

        public ICursor Search(IndexKey key)
        {
            var cursor = _treeTraverser.SearchCursor(key);

            return new IndexCursor(cursor, this);
        }
        
        public ICursor Next()
        {
            if(!_cursor.HasValue) throw new InvalidOperationException("Attempting to advance without a cursor");
            
            var cursor = _treeTraverser.AdvanceCursor(_cursor.Value!);

            return new IndexCursor(cursor, this);
        }

        public int Key()
        {
            throw new NotImplementedException();
        }

        public ColumnValue Column(int columnIndex)
        {
            if(!_cursor.HasValue) throw new InvalidOperationException("Attempting to get heap without a cursor");
            var cursor = _cursor.Value!;
            
            var page = _tableManager.Pager.Get(cursor.Page);
            var leaf = LeafNode.Read(page, _treeSerializer);

            if (columnIndex == 0)
            {
                var cellValue = leaf.GetCellValue(cursor.CellNumber);

                return cellValue.Values[0];
            }

            // shift to account for rowid
            columnIndex -= 1;
            
            var keyCount = Index.Structure.Keys.Count;
            if (columnIndex < keyCount)
            {
                var cellKey = leaf.GetCellKey(cursor.CellNumber);

                return cellKey.Values[columnIndex];
            }

            // unshift since rowid is actually in the data
            columnIndex += 1;
            columnIndex -= keyCount;
            
            if (columnIndex < Index.Structure.Data.Count + 1)
            {
                var cellValue = leaf.GetCellValue(cursor.CellNumber);

                return cellValue.Values[columnIndex];
            }

            columnIndex += keyCount;

            throw new Exception($"invalid column index: {columnIndex} for {Index.Name}");
        }

        public InsertTargetResult Insert(Row row)
        {
            var inserter = new TableInserter(_tableManager);
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
            var deleter = new TableDeleter(_tableManager, _txm);
            var result = deleter.Delete(GetHeap());

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

        private HeapCursor GetHeap()
        {
            if(!_cursor.HasValue) throw new InvalidOperationException("Attempting to get heap without a cursor");
            var cursor = _cursor.Value!;
            
            var page = _tableManager.Pager.Get(cursor.Page);
            var leaf = LeafNode.Read(page, _treeSerializer);

            // (pageIndex << 8) | itemIndex
            var heapLocation = (int?)leaf.GetCellValue(cursor.CellNumber).Values.First().Value ?? throw new Exception("???");

            return HeapCursor.FromLocation(_tableManager, _txm, Writable, heapLocation);
        }
    }
}
