namespace SimpleDatabase.Planning.Nodes;

public class ExplainNode : Node
{
    public Node Node { get; }

    public ExplainNode(string alias, Node node) : base(alias)
    {
        Node = node;
    }
}