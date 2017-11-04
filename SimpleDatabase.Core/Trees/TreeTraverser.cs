using System;
using SimpleDatabase.Core.Paging;

namespace SimpleDatabase.Core.Trees
{
    public class TreeTraverser
    {
        private readonly Table _table;
        private readonly IPager _pager;

        public TreeTraverser(Table table, IPager pager)
        {
            _table = table;
            _pager = pager;
        }

        public Cursor StartCursor()
        {
            var seacher = new TreeStartSearcher(_pager);

            return seacher.FindCursor(_table.RootPageNumber);
        }

        public Cursor AdvanceCursor(Cursor cursor)
        {
            if (cursor.EndOfTable)
            {
                throw new InvalidOperationException("End of table - cannot advance");
            }

            var page = _pager.Get(cursor.PageNumber);
            var node = LeafNode.Read(page);

            var cellNumber = cursor.CellNumber + 1;

            if (cellNumber < node.CellCount)
            {
                return CreateCursor(
                    cursor.PageNumber,
                    cellNumber,
                    node
                );
            }

            var nextPageNumber = node.NextLeaf;
            if (nextPageNumber == 0)
            {
                return new Cursor(
                    cursor.PageNumber,
                    cellNumber,
                    true
                );
            }

            var nextPage = _pager.Get(nextPageNumber);
            var nextNode = LeafNode.Read(nextPage);

            return CreateCursor(
                nextPageNumber,
                0,
                nextNode
            );
        }

        public static Cursor CreateCursor(int pageNumber, int cellNumber, LeafNode node)
        {
            var lastCell = cellNumber >= node.CellCount;
            var noNextLeaf = node.NextLeaf == 0;

            return new Cursor(
                pageNumber,
                cellNumber,
                lastCell && noNextLeaf
            );
        }
    }
}
