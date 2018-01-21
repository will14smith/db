using System;
using SimpleDatabase.Execution.Transactions;
using SimpleDatabase.Execution.Trees;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Heap;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;
using SimpleDatabase.Storage.Tree;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Execution.Tables
{
    public class TreeTraverser
    {
        private readonly ISourcePager _treePager;
        private readonly ISourcePager _heapPager;
        private readonly ITransactionManager _txm;

        private readonly Index _index;

        private readonly RowSerializer _keySerializer;
        private readonly RowSerializer _dataSerializer;

        public TreeTraverser(ISourcePager treePager, ISourcePager heapPager, ITransactionManager txm, Index index)
        {
            _treePager = treePager;
            _heapPager = heapPager;
            _txm = txm;

            _index = index;

            var (keyColumns, dataColumns) = index.GetPersistenceColumns();

            _keySerializer = new RowSerializer(keyColumns, new ColumnTypeSerializerFactory());
            _dataSerializer = new RowSerializer(dataColumns, new ColumnTypeSerializerFactory());

            
        }

        public Cursor StartCursor()
        {
            var strategy = new TreeStartSearcher();
            var seacher = new TreeSearcher(_treePager, strategy, _index);

            var cursor = seacher.FindCursor(_index.RootPage);

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

            var page = _treePager.Get(cursor.Page.Index);
            var node = LeafNode.Read(page, _keySerializer, _dataSerializer);

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
                new PageId(_treePager.Source, nextPageNumber), 
                0,
                false
            );
        }
        
        private bool IsVisible(Cursor cursor)
        {
            var leaf = LeafNode.Read(_treePager.Get(cursor.Page.Index), _keySerializer, _dataSerializer);
            var cell = leaf.GetCellOffset(cursor.CellNumber);

            // TODO wrap in an accessor class
            var heapPage = cell.Slice(0).Read<int>();
            var heapCell = cell.Slice(sizeof(int)).Read<int>();

            var page = HeapPage.Read(_heapPager.Get(heapPage));
            var rowStart = page.GetItem(heapCell);

            var (min, maxopt) = RowSerializer.ReadXid(rowStart);

            return _txm.IsVisible(min, maxopt);
        }
    }
}
