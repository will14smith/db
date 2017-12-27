using SimpleDatabase.Execution.Transactions;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Heap;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;

namespace SimpleDatabase.Execution.Tables
{
    public class HeapTraverser
    {
        private readonly ISourcePager _pager;
        private readonly ITransactionManager _txm;
        private IRowSerializer _rowSerializer;
        private readonly Table _table;
            
        public HeapTraverser(ISourcePager pager, ITransactionManager txm, IRowSerializer rowSerializer, Table table)
        {
            _pager = pager;
            _txm = txm;
            _rowSerializer = rowSerializer;
            _table = table;
        }

        public Cursor StartCursor()
        {
            var pageId = new PageId(_pager.Source, 0);
            var page = HeapPage.Read(_pager.Get(0));

            var tableIsEmpty = page.ItemCount == 0;

            var cursor = new Cursor(pageId, 0, tableIsEmpty);

            return AdvanceUntilVisible(cursor);
        }

        public Cursor AdvanceCursor(Cursor cursor)
        {
            cursor = AdvanceToNext(cursor);

            return AdvanceUntilVisible(cursor);
        }

        private Cursor AdvanceUntilVisible(Cursor cursor)
        {
            while (!cursor.EndOfTable && !IsVisible(cursor))
            {
                cursor = AdvanceToNext(cursor);
            }

            return cursor;
        }

        private bool IsVisible(Cursor cursor)
        {
            var page = HeapPage.Read(_pager.Get(cursor.Page.Index));
            var rowStart = page.GetItem(cursor.CellNumber);

            var tx = _txm.Current.Id;

            var (min, maxopt) = _rowSerializer.ReadXid(rowStart);

            if (maxopt.HasValue)
            {
                var max = maxopt.Value;

                if (max < tx && _txm.IsCommitted(max))
                {
                    // has been deleted
                    return false;
                }
                if (max == tx)
                {
                    // has been deleted by this tx
                    return false;
                }
            }

            if (min < tx && _txm.IsCommitted(min))
            {
                return true;
            }
            if (min == tx)
            {
                // inserted by this tx
                return true;
            }


            return false;
        }
        
        private Cursor AdvanceToNext(Cursor cursor)
        {
            var pageId = cursor.Page;
            var cell = cursor.CellNumber + 1;

            var page = HeapPage.Read(_pager.Get(pageId.Index));

            if (cell < page.ItemCount)
            {
                return new Cursor(pageId, cell, false);
            }

            if (page.NextPageIndex == 0)
            {
                return new Cursor(pageId, cell, true);
            }

            pageId = new PageId(pageId.Source, page.NextPageIndex);
            cell = 0;

            page = HeapPage.Read(_pager.Get(pageId.Index));

            var pageIsEmpty = page.ItemCount == 0;

            return new Cursor(pageId, cell, pageIsEmpty);
        }
    }
}
