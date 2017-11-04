using System;
using SimpleDatabase.Core.Paging;

namespace SimpleDatabase.Core.Trees
{
    public class TreeSearcher
    {
        private readonly IPager _pager;

        public TreeSearcher(IPager pager)
        {
            _pager = pager;
        }

        public Cursor FindCursor(int pageNumber, int key)
        {
            var page = _pager.Get(pageNumber);
            var node = Node.Read(page);

            switch (node)
            {
                case LeafNode leafNode:
                    return LeafNodeFind(leafNode, pageNumber, key);
                case InternalNode internalNode:
                    return InternalNodeFind(internalNode, key);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Cursor LeafNodeFind(LeafNode node, int pageNumber, int key)
        {
            var minIndex = 0;
            var onePastMaxIndex = node.CellCount;
            while (onePastMaxIndex != minIndex)
            {
                var index = (minIndex + onePastMaxIndex) / 2;
                var keyAtIndex = node.GetCellKey(index);
                if (key == keyAtIndex)
                {
                    minIndex = index;
                    break;
                }

                if (key < keyAtIndex)
                {
                    onePastMaxIndex = index;
                }
                else
                {
                    minIndex = index + 1;
                }
            }

            return new Cursor(
                pageNumber,
                minIndex,
                minIndex >= node.CellCount
            );
        }

        private Cursor InternalNodeFind(InternalNode node, int key)
        {
            var minIndex = 0;
            var maxIndex = node.KeyCount; // there is one more child than key

            while (minIndex != maxIndex)
            {
                var index = (minIndex + maxIndex) / 2;
                var keyToRight = node.GetKey(index);
                if (keyToRight >= key)
                {
                    maxIndex = index;
                }
                else
                {
                    minIndex = index + 1;
                }
            }

            var childPageNumber = node.GetChild(minIndex);
            return FindCursor(childPageNumber, key);
        }


    }
}
