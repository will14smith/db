﻿using SimpleDatabase.Execution.Transactions;
using SimpleDatabase.Storage.Heap;
using SimpleDatabase.Storage.MetaData;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;
using SimpleDatabase.Storage.TableMetaData;

namespace SimpleDatabase.Execution.Tables
{
    public class HeapTraverser
    {
        private readonly ISourcePager _pager;
        private readonly ITransactionManager _txm;
            
        public HeapTraverser(ISourcePager pager, ITransactionManager txm)
        {
            _pager = pager;
            _txm = txm;
        }

        public Cursor StartCursor()
        {
            var metaDataPage = TableMetaDataPage.Read(_pager.Get(0)); 

            var pageId = new PageId(_pager.Source, metaDataPage.RootHeapPageIndex);
            var page = HeapPage.Read(_pager.Get(pageId));

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

            var (min, maxopt) = HeapSerializer.ReadXid(rowStart);

            return _txm.IsVisible(min, maxopt);
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
