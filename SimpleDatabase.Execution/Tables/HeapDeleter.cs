using System;
using SimpleDatabase.Execution.Transactions;
using SimpleDatabase.Execution.Values;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Heap;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;

namespace SimpleDatabase.Execution.Tables
{
    public class HeapDeleter
    {
        private readonly ISourcePager _pager;
        private readonly ITransactionManager _txm;


        public HeapDeleter(ISourcePager pager, ITransactionManager txm)
        {
            _pager = pager;
            _txm = txm;
        }

        public DeleteResult Delete(HeapCursor heapCursor)
        {
            var cursor = heapCursor.Cursor.Value;
            var page = HeapPage.Read(_pager.Get(cursor.Page.Index));
            var row = page.GetItem(cursor.CellNumber);

            HeapSerializer.SetXidMax(row, _txm.Current.Id);
            _pager.Flush(page.PageId.Index);

            return new DeleteResult.Success();
        }
    }
}