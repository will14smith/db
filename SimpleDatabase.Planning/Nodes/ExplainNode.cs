namespace SimpleDatabase.Planning.Nodes;

public class ExplainNode : Node
{
    public Node Node { get; }

    public ExplainNode(Node node)
    {
        Node = node;
    }
}