using System;
using SimpleDatabase.Core.Paging;

namespace SimpleDatabase.Core.Trees
{
    public class TreeSearcher
    {
        private readonly IPager _pager;
        private readonly ITreeSearchStrategy _treeSearchStrategy;

        public TreeSearcher(IPager pager, ITreeSearchStrategy treeSearchStrategy)
        {
            _pager = pager;
            _treeSearchStrategy = treeSearchStrategy;
        }

        public Cursor FindCursor(int pageNumber)
        {
            var page = _pager.Get(pageNumber);
            var node = Node.Read(page);

            switch (node)
            {
                case LeafNode leafNode:
                    return LeafNodeFind(leafNode, pageNumber);
                case InternalNode internalNode:
                    return InternalNodeFind(internalNode);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Cursor LeafNodeFind(LeafNode node, int pageNumber)
        {
            var cellNumber = _treeSearchStrategy.FindCell(node);

            return TreeTraverser.CreateCursor(pageNumber, cellNumber, node);
        }

        private Cursor InternalNodeFind(InternalNode node)
        {
            var minIndex = _treeSearchStrategy.FindCell(node);

            var childPageNumber = node.GetChild(minIndex);

            return FindCursor(childPageNumber);
        }
    }
}