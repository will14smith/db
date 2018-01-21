using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;

namespace SimpleDatabase.Execution.Tables
{
    public class TableDeleter
    {
        private readonly IPager _pager;
        private readonly Table _table;

        public TableDeleter(IPager pager, Table table)
        {
            _pager = pager;
            _table = table;
        }

        public DeleteResult Delete(Cursor cursor)
        {
            // TODO check cursor.Table == _table

            var heapDeleter = new HeapDeleter(new SourcePager(_pager, new PageSource.Heap(_table.Name)), _table);
            var result = heapDeleter.Delete(cursor);
            // TODO check result, abort if failed....

            foreach (var index in _table.Indices)
            {
                // TODO create serializer for index row
                var treeDeleter = new TreeDeleter(new SourcePager(_pager, new PageSource.Index(_table.Name, index.Name)), index);

                // TODO get key value from row
                var key = 0;

                result = treeDeleter.Delete(key);
                // TODO check result, abort if failed....
            }

            return result;
        }
    }
}
