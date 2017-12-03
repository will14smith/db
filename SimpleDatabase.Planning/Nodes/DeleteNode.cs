namespace SimpleDatabase.Planning.Nodes
{
    public class DeleteNode : Node
    {
        public Node Input { get; }

        public DeleteNode(Node input)
        {
            Input = input;
        }
    }
}
