using System.Linq;
using SimpleDatabase.Execution.Transactions;
using SimpleDatabase.Execution.Values;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;

namespace SimpleDatabase.Execution.Tables
{
    public class TableDeleter
    {
        private readonly IPager _pager;
        private readonly ITransactionManager _txm;
        private readonly Table _table;

        public TableDeleter(IPager pager, ITransactionManager txm, Table table)
        {
            _pager = pager;
            _txm = txm;
            _table = table;
        }

        public DeleteResult Delete(HeapCursor cursor)
        {
            // TODO check cursor.Table == _table

            DeleteResult result;
            foreach (var index in _table.Indices)
            {
                var treeDeleter = new TreeDeleter(new SourcePager(_pager, new PageSource.Index(_table.Name, index.Name)), index);

                var key = GetKey(index, cursor);

                result = treeDeleter.Delete(key);
                // TODO check result, abort if failed....
            }

            var heapDeleter = new HeapDeleter(new SourcePager(_pager, new PageSource.Heap(_table.Name)), _txm);
            result = heapDeleter.Delete(cursor);
            // TODO check result, abort if failed....

            return result;
        }

        private IndexKey GetKey(TableIndex index, ICursor cursor)
        {
            var key = index.Structure.Keys
                .Select(col => _table.IndexOf(col.Item1) ?? -1)
                .Select(cursor.Column)
                .ToList();

            return new IndexKey(key);
        }
    }
}
