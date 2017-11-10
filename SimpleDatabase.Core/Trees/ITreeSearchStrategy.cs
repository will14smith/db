namespace SimpleDatabase.Core.Trees
{
    public interface ITreeSearchStrategy
    {
        int FindCell(LeafNode node);
        int FindCell(InternalNode node);
    }
}