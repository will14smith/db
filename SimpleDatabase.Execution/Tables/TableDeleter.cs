using System;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Paging;

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
            throw new NotImplementedException();
        }
    }
}
