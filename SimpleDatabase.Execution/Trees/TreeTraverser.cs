using System;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;
using SimpleDatabase.Storage.Tree;

namespace SimpleDatabase.Execution.Trees
{
    public class TreeTraverser
    {
        private readonly ISourcePager _pager;
        private readonly IRowSerializer _rowSerializer;

        private readonly Index _index;

        public TreeTraverser(ISourcePager pager, IRowSerializer rowSerializer, Index index)
        {
            _pager = pager;
            _rowSerializer = rowSerializer;
            _index = index;
        }

        public Cursor StartCursor()
        {
            var strategy = new TreeStartSearcher();
            var seacher = new TreeSearcher(_pager, strategy, _rowSerializer);

            return seacher.FindCursor(_index.RootPage);
        }

        public Cursor AdvanceCursor(Cursor cursor)
        {
            if (cursor.EndOfTable)
            {
                throw new InvalidOperationException("End of table - cannot advance");
            }

            var page = _pager.Get(cursor.Page.Index);
            var node = LeafNode.Read(_rowSerializer, page);

            var cellNumber = cursor.CellNumber + 1;

            if (cellNumber < node.CellCount)
            {
                var lastCell = cellNumber >= node.CellCount;
                var noNextLeaf = node.NextLeaf == 0;

                return new Cursor(
                    cursor.Page,
                    cellNumber,
                    lastCell && noNextLeaf
                );
            }

            var nextPageNumber = node.NextLeaf;
            if (nextPageNumber == 0)
            {
                return new Cursor(
                    cursor.Page,
                    cellNumber,
                    true
                );
            }

            return new Cursor(
                new PageId(_pager.Source, nextPageNumber), 
                0,
                false
            );
        }
    }
}
