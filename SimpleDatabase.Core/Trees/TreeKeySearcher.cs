using SimpleDatabase.Core.Paging;

namespace SimpleDatabase.Core.Trees
{
    public class TreeKeySearcher : BaseTreeSearcher
    {
        private readonly int _key;

        public TreeKeySearcher(IPager pager, int key) : base(pager)
        {
            _key = key;
        }

        public override int LeafNodeFindCell(LeafNode node)
        {
            var minIndex = 0;
            var onePastMaxIndex = node.CellCount;
            while (onePastMaxIndex != minIndex)
            {
                var index = (minIndex + onePastMaxIndex) / 2;
                var keyAtIndex = node.GetCellKey(index);
                if (_key == keyAtIndex)
                {
                    minIndex = index;
                    break;
                }

                if (_key < keyAtIndex)
                {
                    onePastMaxIndex = index;
                }
                else
                {
                    minIndex = index + 1;
                }
            }
            return minIndex;
        }

        public override int InternalNodeFindCell(InternalNode node)
        {
            var minIndex = 0;
            var maxIndex = node.KeyCount; // there is one more child than key

            while (minIndex != maxIndex)
            {
                var index = (minIndex + maxIndex) / 2;
                var keyToRight = node.GetKey(index);
                if (keyToRight >= _key)
                {
                    maxIndex = index;
                }
                else
                {
                    minIndex = index + 1;
                }
            }
            return minIndex;
        }
    }
}
