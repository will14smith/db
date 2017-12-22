using System;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;

namespace SimpleDatabase.Execution.Tables
{
    public class TableInserter
    {
        private readonly IPager _pager;
        private readonly Table _table;

        private readonly IRowSerializer _rowSerializer;

        public TableInserter(IPager pager, Table table)
        {
            _pager = pager;
            _table = table;

            _rowSerializer = new RowSerializer(table, new ColumnTypeSerializerFactory());
        }

        public InsertResult Insert(Row row)
        {
            var heapInserter = new HeapInserter(new SourcePager(_pager, new PageSource.Heap(_table.Name)), _table);
            var result = heapInserter.Insert(row);
            // TODO check result

            foreach (var index in _table.Indices)
            {
                // TODO get key from row
                // TODO create virtual "table" with heap key & index data columns
                var key = 0;

                var treeInserter = new TreeInserter(new SourcePager(_pager, new PageSource.Index(_table.Name, index.Name)), _rowSerializer, index);
                result = treeInserter.Insert(key, row);
                // TODO check result, rollback all inserts (heap & index) if no success...
            }

            throw new NotImplementedException();
        }
    }
}
