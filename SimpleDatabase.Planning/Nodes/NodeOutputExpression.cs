using System;
using SimpleDatabase.Parsing.Expressions;

namespace SimpleDatabase.Planning.Nodes;

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