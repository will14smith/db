using System;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Paging;

namespace SimpleDatabase.Execution.Tables
{
    public class HeapTraverser
    {
        private readonly ISourcePager _pager;
        private readonly Table _table;

        public HeapTraverser(ISourcePager pager, Table table)
        {
            _pager = pager;
            _table = table;
        }
        
        public Cursor StartCursor()
        {
            throw new NotImplementedException();
        }

        public Cursor AdvanceCursor(Cursor cursor)
        {
            throw new NotImplementedException();
        }
    }
}
