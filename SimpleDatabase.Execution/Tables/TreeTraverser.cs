using System;
using SimpleDatabase.Execution.Transactions;
using SimpleDatabase.Execution.Trees;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage;
using SimpleDatabase.Storage.Heap;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;
using SimpleDatabase.Storage.Tree;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Execution.Tables
{
    public class TreeTraverser
    {
        private readonly TableManager _tableManager;
        private readonly ITransactionManager _txm;

        private readonly IIndexSerializer _treeSerializer;
        
        private readonly TableIndex _index;
        
        public TreeTraverser(TableManager tableManager, ITransactionManager txm, TableIndex index)
        {
            _tableManager = tableManager;
            _txm = txm;

            _index = index;

            _treeSerializer = index.CreateSerializer();
        }

        public Cursor StartCursor()
        {
            var strategy = new TreeStartSearcher();
            var seacher = new TreeSearcher(_tableManager.Pager, strategy, _index);

            var cursor = seacher.FindCursor(_tableManager.GetIndexRootPageId(_index));

            return AdvanceUntilVisible(cursor);
        }
        
        public Cursor SearchCursor(IndexKey key)
        {
            var strategy = new TreeKeySearcher(key);
            var seacher = new TreeSearcher(_tableManager.Pager, strategy, _index);

            var cursor = seacher.FindCursor(_tableManager.GetIndexRootPageId(_index));

            return AdvanceUntilVisible(cursor);
        }

        public Cursor AdvanceCursor(Cursor cursor)
        {
            cursor = AdvanceToNext(cursor);

            return AdvanceUntilVisible(cursor);
        }

        private Cursor AdvanceUntilVisible(Cursor cursor)
        {
            while (!cursor.EndOfTable && !IsVisible(cursor))
            {
                cursor = AdvanceToNext(cursor);
            }

            return cursor;
        }

        private Cursor AdvanceToNext(Cursor cursor)
        {
            if (cursor.EndOfTable)
            {
                throw new InvalidOperationException("End of table - cannot advance");
            }

            var page = _tableManager.Pager.Get(cursor.Page.Index);
            var node = LeafNode.Read(page, _treeSerializer);

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
                new PageId(_tableManager.Pager.Source, nextPageNumber), 
                0,
                false
            );
        }
        
        private bool IsVisible(Cursor cursor)
        {
            var leaf = LeafNode.Read(_tableManager.Pager.Get(cursor.Page), _treeSerializer);
            var cell = leaf.GetCellValueOffset(cursor.CellNumber);

            // TODO wrap in an accessor class
            var heapKey = cell.Slice(0).Read<int>();
            var heapPage = heapKey >> 8;
            var heapCell = heapKey & 0xff;

            var page = HeapPage.Read(_tableManager.Pager.Get(heapPage));
            var rowStart = page.GetItem(heapCell);

            var (min, maxopt) = HeapSerializer.ReadXid(rowStart);

            return _txm.IsVisible(min, maxopt);
        }
    }
}
