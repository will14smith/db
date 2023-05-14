using System;
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

public class RowIdLookupNode : Node
{
    public string TableName { get; }
    public Expression RowId { get; }

    public RowIdLookupNode(string alias, string tableName, Expression rowId) : base(alias)
    {
        TableName = tableName;
        RowId = rowId;
    }
}

public class NodeOutputExpression : Expression
{
    public string NodeAlias { get; }
    public int Index { get; }

    public NodeOutputExpression(string nodeAlias, int index)
    {
        NodeAlias = nodeAlias;
        Index = index;
    }

    public override string ToString()
    {
        return $"{NodeAlias}[{Index}]";
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        
        return obj is NodeOutputExpression other && NodeAlias == other.NodeAlias && Index == other.Index;
    }

    public override int GetHashCode() => HashCode.Combine(NodeAlias, Index);
}