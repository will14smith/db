using SimpleDatabase.Storage.Nodes;

namespace SimpleDatabase.Execution.Trees
{
    public interface ITreeSearchStrategy
    {
        int FindCell(LeafNode node);
        int FindCell(InternalNode node);
    }
}