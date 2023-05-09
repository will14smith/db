using System;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage;
using SimpleDatabase.Storage.Heap;
using SimpleDatabase.Storage.MetaData;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;
using SimpleDatabase.Storage.TableMetaData;

namespace SimpleDatabase.Execution.Tables
{
    public class HeapInserter
    {
        private readonly ISourcePager _pager;

        private readonly IHeapSerializer _rowSerializer;

        public HeapInserter(TableManager tableManager)
        {
            _pager = tableManager.Pager;

            _rowSerializer = new HeapSerializer(tableManager.Table.Columns, new ColumnTypeSerializerFactory());
        }

        public int Insert(Row row)
        {
            // find the last page
            var metaDataPage = TableMetaDataPage.Read(_pager.Get(0)); 
            var page = HeapPage.Read(_pager.Get(metaDataPage.RootHeapPageIndex));
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

            return (pageIndex << 8) | itemIndex;
        }
    }
}