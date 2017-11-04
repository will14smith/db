using SimpleDatabase.Core.Paging;

namespace SimpleDatabase.Core.Trees
{
    public class TreeStartSearcher : BaseTreeSearcher
    {
        public TreeStartSearcher(IPager pager) : base(pager)
        {
        }

        public override int LeafNodeFindCell(LeafNode node)
        {
            return 0;
        }

        public override int InternalNodeFindCell(InternalNode node)
        {
            return 0;
        }
    }
}