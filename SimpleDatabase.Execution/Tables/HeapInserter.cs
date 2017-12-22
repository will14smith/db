using System;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Heap;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;

namespace SimpleDatabase.Execution.Tables
{
    public class HeapInserter
    {
        private readonly ISourcePager _pager;

        private readonly IRowSerializer _rowSerializer;

        public HeapInserter(ISourcePager pager, Table table)
        {
            _pager = pager;

            _rowSerializer = new RowSerializer(table, new ColumnTypeSerializerFactory());
        }

        public InsertResult Insert(Row row)
        {
            // find the last page
            var page = HeapPage.Read(_pager.Get(0));
            while (page.NextPageIndex != 0)
            {
                page = HeapPage.Read(_pager.Get(page.NextPageIndex));
            }

            var rowSize = _rowSerializer.GetRowSize();
            if (!page.CanInsert(rowSize))
            {
                throw new NotImplementedException("page is full, create a new one");
            }
            
            // insert into page
            var itemIndex = page.AddItem(rowSize, dst => _rowSerializer.WriteRow(dst, row));
            var pageIndex = page.PageId.Index;

            if (pageIndex > 0xffffff)
            {
                // TODO add result type?
                throw new IndexOutOfRangeException("Page index will overflow");
            }

            var key = (pageIndex << 8) | itemIndex;

            return new InsertResult.Success(key);
        }
    }
}