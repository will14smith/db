using System;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Paging;

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
            throw new NotImplementedException();
        }
    }
}
