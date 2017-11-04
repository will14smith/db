using System;
using SimpleDatabase.Core.Paging;

namespace SimpleDatabase.Core.Trees
{
    public abstract class BaseTreeSearcher
    {
        private readonly IPager _pager;

        protected BaseTreeSearcher(IPager pager)
        {
            _pager = pager;
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
            var cellNumber = LeafNodeFindCell(node);

            return TreeTraverser.CreateCursor(pageNumber, cellNumber, node);
        }

        public abstract int LeafNodeFindCell(LeafNode node);

        private Cursor InternalNodeFind(InternalNode node)
        {
            var minIndex = InternalNodeFindCell(node);

            var childPageNumber = node.GetChild(minIndex);

            return FindCursor(childPageNumber);
        }

        public abstract int InternalNodeFindCell(InternalNode node);
    }
}