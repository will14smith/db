using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Heap;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Tree;

namespace SimpleDatabase.Storage
{
    public class TableCreator
    {
        private readonly IPager _pager;

        public TableCreator(IPager pager)
        {
            _pager = pager;
        }

        public void Create(Table table)
        {
            var heapPager = new SourcePager(_pager, new PageSource.Heap(table.Name));
            new HeapCreator(heapPager, table).Create();

            foreach (var index in table.Indexes)
            {
                var indexPager = new SourcePager(_pager, new PageSource.Index(table.Name, index.Name));
                new IndexCreator(indexPager, index).Create();
            }
        }
    }
}