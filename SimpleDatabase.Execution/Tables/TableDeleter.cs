using System.Linq;
using SimpleDatabase.Execution.Transactions;
using SimpleDatabase.Execution.Values;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;

namespace SimpleDatabase.Execution.Tables
{
    public class TableDeleter
    {
        private readonly TableManager _tableManager;
        private readonly ITransactionManager _txm;

        public TableDeleter(TableManager tableManager, ITransactionManager txm)
        {
            _tableManager = tableManager;
            _txm = txm;
        }

        public DeleteResult Delete(HeapCursor cursor)
        {
            // TODO check cursor.Table == _table

            DeleteResult result;
            foreach (var index in _tableManager.Table.Indexes)
            {
                var treeDeleter = new TreeDeleter(_tableManager, index);

                var key = GetKey(index, cursor);

                result = treeDeleter.Delete(key);
                // TODO check result, abort if failed....
            }

            var heapDeleter = new HeapDeleter(_tableManager.Pager, _txm);
            result = heapDeleter.Delete(cursor);
            // TODO check result, abort if failed....

            return result;
        }

        private IndexKey GetKey(TableIndex index, ICursor cursor)
        {
            var key = index.Structure.Keys
                .Select(col => _tableManager.Table.IndexOf(col.Item1) ?? -1)
                .Select(cursor.Column)
                .ToList();

            return new IndexKey(key);
        }
    }
}
