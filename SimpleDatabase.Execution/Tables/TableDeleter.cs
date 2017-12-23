using System;
using SimpleDatabase.Execution.Values;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;

namespace SimpleDatabase.Execution.Tables
{
    public class TableDeleter
    {
        private readonly IPager _pager;
        private readonly Table _table;

        private readonly IRowSerializer _rowSerializer;

        public TableDeleter(IPager pager, Table table)
        {
            _pager = pager;
            _table = table;

            _rowSerializer = new RowSerializer(table, new ColumnTypeSerializerFactory());
        }

        public DeleteResult Delete(Cursor cursor)
        {
            // TODO check cursor.Table == _table

            var heapDeleter = new HeapDeleter(new SourcePager(_pager, new PageSource.Heap(_table.Name)), _table);
            var result = heapDeleter.Delete(cursor);
            // TODO check result

            foreach (var index in _table.Indices)
            {
                // TODO calculate key
                var key = 0;

                var treeDeleter = new TreeDeleter(new SourcePager(_pager, new PageSource.Index(_table.Name, index.Name)), _rowSerializer, index);
                result = treeDeleter.Delete(key);
                // TODO check result, rollback all inserts (heap & index) if no success...
            }

            return result;
        }
    }
}
