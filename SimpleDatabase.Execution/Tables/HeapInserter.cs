using System;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Paging;

namespace SimpleDatabase.Execution.Tables
{
    public class HeapInserter
    {
        private readonly ISourcePager _pager;
        private readonly Table _table;

        public HeapInserter(ISourcePager pager, Table table)
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