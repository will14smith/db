﻿using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Heap;
using SimpleDatabase.Storage.Paging;

namespace SimpleDatabase.Execution.Tables
{
    public class HeapTraverser
    {
        private readonly ISourcePager _pager;
        private readonly Table _table;

        public HeapTraverser(ISourcePager pager, Table table)
        {
            _pager = pager;
            _table = table;
        }

        public Cursor StartCursor()
        {
            var pageId = new PageId(_pager.Source, 0);
            var page = HeapPage.Read(_pager.Get(0));

            var tableIsEmpty = page.ItemCount == 0;

            return new Cursor(pageId, 0, tableIsEmpty);
        }

        public Cursor AdvanceCursor(Cursor cursor)
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
