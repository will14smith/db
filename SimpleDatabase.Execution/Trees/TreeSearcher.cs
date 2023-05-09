using System;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;
using SimpleDatabase.Storage.Tree;

namespace SimpleDatabase.Execution.Trees
{
    public class TreeSearcher
    {
        private readonly ISourcePager _pager;
        private readonly ITreeSearchStrategy _treeSearchStrategy;
        private readonly IIndexSerializer _serializer;

        public TreeSearcher(ISourcePager pager, ITreeSearchStrategy treeSearchStrategy, TableIndex index)
        {
            _pager = pager;
            _treeSearchStrategy = treeSearchStrategy;
            _serializer = index.CreateSerializer();
        }

        public Cursor FindCursor(PageId pageIndex)
        {
            var page = _pager.Get(pageIndex);
            var node = Node.Read(page, _serializer);

            switch (node)
            {
                case LeafNode leafNode:
                    return LeafNodeFind(leafNode, pageIndex);
                case InternalNode internalNode:
                    return InternalNodeFind(internalNode);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Cursor LeafNodeFind(LeafNode node, PageId pageIndex)
        {
            var cellNumber = _treeSearchStrategy.FindCell(node);

            var lastCell = cellNumber >= node.CellCount;
            var noNextLeaf = node.NextLeaf == 0;

            return new Cursor(
                pageIndex,
                cellNumber,
                lastCell && noNextLeaf
            );
        }

        private Cursor InternalNodeFind(InternalNode node)
        {
            var minIndex = _treeSearchStrategy.FindCell(node);

            var childPageNumber = node.GetChild(minIndex);

            return FindCursor(new PageId(node.PageId.Source, childPageNumber));
        }
    }
}