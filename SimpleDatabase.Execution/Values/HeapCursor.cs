using System;
using SimpleDatabase.Execution.Tables;
using SimpleDatabase.Execution.Transactions;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage;
using SimpleDatabase.Storage.Heap;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Execution.Values
{
    public class HeapCursor : ICursor, IInsertTarget, IDeleteTarget
    {
        public Option<Cursor> Cursor { get; }

        private readonly TableManager _tableManager;
        private readonly ITransactionManager _txm;

        private readonly IHeapSerializer _heapSerializer;

        public bool Writable { get; }

        public HeapCursor(TableManager tableManager, ITransactionManager txm, bool writable)
        {
            _tableManager = tableManager;
            _txm = txm;

            _heapSerializer = new HeapSerializer(_tableManager.Table.Columns, new ColumnTypeSerializerFactory());

            Writable = writable;
        }
        public HeapCursor(Cursor cursor, HeapCursor heapCursor)
            : this(heapCursor._tableManager, heapCursor._txm, heapCursor.Writable)
        {
            Cursor = Option.Some(cursor);
        }

        public bool EndOfTable => Cursor.HasValue && Cursor.Value!.EndOfTable;

        public ICursor First()
        {
            var traverser = new HeapTraverser(_tableManager.Pager, _txm);
            var cursor = traverser.StartCursor();

            return new HeapCursor(cursor, this);
        }

        public ICursor Next()
        {
            if (!Cursor.HasValue)
            {
                throw new InvalidOperationException("Attempting to advance without a cursor");
            }
            
            var traverser = new HeapTraverser(_tableManager.Pager, _txm);
            var cursor = traverser.AdvanceCursor(Cursor.Value!);

            return new HeapCursor(cursor, this);
        }

        public int Key()
        {
            throw new NotImplementedException();
        }

        public ColumnValue Column(int index)
        {
            if(!Cursor.HasValue) throw new InvalidOperationException("Cannot read column without a cursor");
            var cursor = Cursor.Value!;

            var page = HeapPage.Read(_tableManager.Pager.Get(cursor.Page));
            var cell = page.GetItem(cursor.CellNumber);

            return _heapSerializer.ReadColumn(cell, index);
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
            // find next cursor
            var next = Next();

            var deleter = new TableDeleter(_tableManager, _txm);
            var result = deleter.Delete(this);

            switch (result)
            {
                case DeleteResult.Success _:
                    return new DeleteTargetResult.Success(next);
                    
                case DeleteResult.KeyNotFound _:
                    throw new NotImplementedException("TODO error handling!");

                default:
                    throw new NotImplementedException($"Unsupported type: {result.GetType().Name}");
            }
        }
        
        public HeapCursor MoveToLocation(int heapLocation) => FromLocation(_tableManager, _txm, Writable, heapLocation);

        public static HeapCursor FromLocation(TableManager tableManager, ITransactionManager txm, bool writable, int heapLocation)
        {
            var pageIndex = heapLocation >> 8;
            var itemIndex = heapLocation & 0xff;

            var pageId = new PageId(tableManager.Pager.Source, pageIndex);

            var heapPage = tableManager.Pager.Get(pageId);
            var heap = HeapPage.Read(heapPage);

            var endOfTable = itemIndex + 1 == heap.ItemCount && heap.NextPageIndex == 0;

            return new HeapCursor(
                new Cursor(pageId, itemIndex, endOfTable), 
                new HeapCursor(tableManager, txm, writable)
            );
        }
    }
}
