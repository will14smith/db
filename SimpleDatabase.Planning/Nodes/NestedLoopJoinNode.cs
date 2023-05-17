using SimpleDatabase.Parsing.Expressions;

namespace SimpleDatabase.Planning.Nodes;

public class NestedLoopJoinNode : Node
{
    public Node Outer { get; }
    public Node Inner { get; }
    public Expression? Predicate { get; }
    
    // foreach x in Outer:
    //   foreach y in Inner(x):
    //     if predicate(x, y):
    //        yield selector(x, y)

    public NestedLoopJoinNode(string alias, Node outer, Node inner, Expression? predicate) : base(alias)
    {
        Outer = outer;
        Inner = inner;
        Predicate = predicate;
    }
}