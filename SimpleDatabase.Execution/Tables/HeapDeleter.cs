using System;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Paging;

namespace SimpleDatabase.Execution.Tables
{
    public class HeapDeleter
    {
        private readonly ISourcePager _pager;
        private readonly Table _table;

        public HeapDeleter(ISourcePager pager, Table table)
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