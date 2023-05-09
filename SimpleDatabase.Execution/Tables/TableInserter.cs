using System.Collections.Generic;
using System.Linq;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;

namespace SimpleDatabase.Execution.Tables
{
    public class TableInserter
    {
        private readonly TableManager _tableManager;

        public TableInserter(TableManager tableManager)
        {
            _tableManager = tableManager;
        }

        public InsertResult Insert(Row row)
        {
            var heapInserter = new HeapInserter(_tableManager);
            var heapKey = heapInserter.Insert(row);
            // TODO handle result of ^

            InsertResult result = new InsertResult.Success();
            foreach (var index in _tableManager.Table.Indexes)
            {
                var treeInserter = new TreeInserter(_tableManager, index);

                var (key, data) = CreateIndexData(index, heapKey, row);

                result = treeInserter.Insert(key, data);
                // TODO check result, abort if failed...
            }

            return result;
        }

        private (IndexKey, IndexData) CreateIndexData(TableIndex index, int heapKey, Row row)
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
            var colIndex = _tableManager.Table.IndexOf(col) ?? -1;

            return row.Values[colIndex];
        }
    }
}
