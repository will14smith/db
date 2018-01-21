using System;
using System.Collections.Generic;
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

            _rowSerializer = new RowSerializer(table.Columns, new ColumnTypeSerializerFactory());
        }

        // TODO remove this...
        private static int _fakeKey = 0;

        public InsertResult Insert(Row row)
        {
            var heapInserter = new HeapInserter(new SourcePager(_pager, new PageSource.Heap(_table.Name)), _table);
            var result = heapInserter.Insert(row);
            // TODO check result, abort if failed...

            foreach (var index in _table.Indices)
            {
                var treeInserter = new TreeInserter(new SourcePager(_pager, new PageSource.Index(_table.Name, index.Name)), index);
                
                // TODO get key value from row
                var key = _fakeKey++;
                // TODO create row for data
                var indexData = row;

                result = treeInserter.Insert(key, indexData);
                // TODO check result, abort if failed...
            }

            return result;
        }
    }
}
