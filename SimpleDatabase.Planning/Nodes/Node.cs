namespace SimpleDatabase.Planning.Nodes
{
    public abstract class Node
    {
        protected Node(string alias)
        {
            Alias = alias;
        }

        public string Alias { get; }
    }
}
