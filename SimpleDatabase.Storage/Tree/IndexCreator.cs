using System;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Paging;

namespace SimpleDatabase.Storage.Tree
{
    public class IndexCreator
    {
        private readonly SourcePager _pager;
        private readonly Index _index;

        public IndexCreator(SourcePager pager, Index index)
        {
            _pager = pager;
            _index = index;
        }

        public void Create()
        {
            throw new NotImplementedException();
        }
    }
}