namespace SimpleDatabase.Planning.Nodes
{
    public class DeleteNode : Node
    {
        public Node Input { get; }

        public DeleteNode(string alias, Node input) : base(alias)
        {
            Input = input;
        }
    }
}
