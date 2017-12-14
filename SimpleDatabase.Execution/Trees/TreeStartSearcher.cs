using SimpleDatabase.Storage.Tree;

namespace SimpleDatabase.Execution.Trees
{
    public class TreeStartSearcher : ITreeSearchStrategy
    {
        public int FindCell(LeafNode node)
        {
            return 0;
        }

        public int FindCell(InternalNode node)
        {
            return 0;
        }
    }
}