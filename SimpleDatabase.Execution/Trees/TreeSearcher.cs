﻿using System;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;
using SimpleDatabase.Storage.Tree;

namespace SimpleDatabase.Execution.Trees
{
    public class TreeSearcher
    {
        private readonly ISourcePager _pager;
        private readonly ITreeSearchStrategy _treeSearchStrategy;
        private readonly IRowSerializer _rowSerializer;

        public TreeSearcher(ISourcePager pager, ITreeSearchStrategy treeSearchStrategy, IRowSerializer rowSerializer)
        {
            _pager = pager;
            _treeSearchStrategy = treeSearchStrategy;
            _rowSerializer = rowSerializer;
        }

        public Cursor FindCursor(int pageIndex)
        {
            var page = _pager.Get(pageIndex);
            var node = Node.Read(_rowSerializer, page);

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

        private Cursor LeafNodeFind(LeafNode node, int pageIndex)
        {
            var cellNumber = _treeSearchStrategy.FindCell(node);

            var lastCell = cellNumber >= node.CellCount;
            var noNextLeaf = node.NextLeaf == 0;

            return new Cursor(
                new PageId(_pager.Source, pageIndex),
                cellNumber,
                lastCell && noNextLeaf
            );
        }

        private Cursor InternalNodeFind(InternalNode node)
        {
            var minIndex = _treeSearchStrategy.FindCell(node);

            var childPageNumber = node.GetChild(minIndex);

            return FindCursor(childPageNumber);
        }
    }
}