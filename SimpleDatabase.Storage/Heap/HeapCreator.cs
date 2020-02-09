using System;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Paging;

namespace SimpleDatabase.Storage.Heap
{
    public class HeapCreator
    {
        private readonly ISourcePager _pager;
        private readonly Table _table;

        public HeapCreator(ISourcePager pager, Table table)
        {
            _pager = pager;
            _table = table;
        }

        public void Create()
        {
            var page = Allocate();
            HeapPage.New(page);
            _pager.Flush(page.Id.Index);
        }

        private Page Allocate()
        {
            var page = _pager.Allocate();
            if (!(page.Id.Source is PageSource.Heap heapSource))
            {
                throw new Exception("Allocated page wasn't from a heap");
            }

            if (heapSource.TableName != _table.Name)
            {
                throw new Exception("Allocated page was from a different table");
            }

            if (page.Id.Index != 0)
            {
                throw new Exception("This pager has already been initialized");
            }

            return page;
        }
    }
}