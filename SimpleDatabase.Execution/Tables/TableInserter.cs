using System.Collections.Generic;
using System.Linq;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;

namespace SimpleDatabase.Execution.Tables
{
    public class TableInserter
    {
        private readonly IPager _pager;
        private readonly Table _table;

        public TableInserter(IPager pager, Table table)
        {
            _pager = pager;
            _table = table;
        }

        public InsertResult Insert(Row row)
        {
            var heapInserter = new HeapInserter(new SourcePager(_pager, new PageSource.Heap(_table.Name)), _table);
            var heapKey = heapInserter.Insert(row);
            // TODO handle result of ^

            InsertResult result = new InsertResult.Success();
            foreach (var index in _table.Indices)
            {
                var treeInserter = new TreeInserter(new SourcePager(_pager, new PageSource.Index(_table.Name, index.Name)), index);

                var (key, data) = CreateIndexData(index, heapKey, row);

                result = treeInserter.Insert(key, data);
                // TODO check result, abort if failed...
            }

            return result;
        }

        private (IndexKey, IndexData) CreateIndexData(Index index, int heapKey, Row row)
        {
            var key = index.Structure.Keys.Select(col => GetValue(col.Item1, row)).ToList();

            var data = new List<ColumnValue>
            {
                new ColumnValue(heapKey)
            };
            key.AddRange(index.Structure.Data.Select(col => GetValue(col, row)));

            return (new IndexKey(key), new IndexData(data));
        }

        private ColumnValue GetValue(Column col, Row row)
        {
            var colIndex = _table.IndexOf(col) ?? -1;

            return row.Values[colIndex];
        }
    }
}
