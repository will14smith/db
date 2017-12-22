using System;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Paging;

namespace SimpleDatabase.Storage.Heap
{
    public class HeapCreator
    {
        private readonly SourcePager _pager;
        private readonly Table _table;

        public HeapCreator(SourcePager pager, Table table)
        {
            _pager = pager;
            _table = table;
        }

        public void Create()
        {
            var page = HeapPage.New(_pager.Allocate());

            _pager.Flush(page.PageId.Index);
        }
    }
}