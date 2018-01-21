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

            _rowSerializer = new RowSerializer(table.Columns, new ColumnTypeSerializerFactory());
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
                var newPage = HeapPage.New(_pager.Allocate());

                page.NextPageIndex = newPage.PageId.Index;
                _pager.Flush(page.PageId.Index);

                page = newPage;
            }
            
            // insert into page
            void Writer(in Span<byte> dst) => _rowSerializer.WriteRow(dst, row);

            var itemIndex = page.AddItem(rowSize, Writer);
            _pager.Flush(page.PageId.Index);

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