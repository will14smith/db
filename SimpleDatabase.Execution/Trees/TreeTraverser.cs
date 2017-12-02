using System;
using SimpleDatabase.Storage;
using SimpleDatabase.Storage.Nodes;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;

namespace SimpleDatabase.Execution.Trees
{
    public class TreeTraverser
    {
        private readonly IPager _pager;
        private readonly IRowSerializer _rowSerializer;
        private readonly StoredTable _table;

        public TreeTraverser(IPager pager, IRowSerializer rowSerializer, StoredTable table)
        {
            _pager = pager;
            _rowSerializer = rowSerializer;
            _table = table;
        }

        public Cursor StartCursor()
        {
            var strategy = new TreeStartSearcher();
            var seacher = new TreeSearcher(_pager, strategy, _rowSerializer);

            return seacher.FindCursor(_table.RootPageNumber);
        }

        public Cursor AdvanceCursor(Cursor cursor)
        {
            if (cursor.EndOfTable)
            {
                throw new InvalidOperationException("End of table - cannot advance");
            }

            var page = _pager.Get(cursor.PageNumber);
            var node = LeafNode.Read(_rowSerializer, page);

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
            var nextNode = LeafNode.Read(_rowSerializer, nextPage);

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
