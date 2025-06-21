using System.Collections.Generic;
using SimpleDatabase.Schemas;

namespace SimpleDatabase.Planning.Optimisation.Algebra;

public abstract record LogicalNode : Node
{
    public record Get(string Alias, Table Table, AlgebraExpression? Predicate = null) : LogicalNode
    {
        public override string ToString()
        {
            if (Alias != Table.Name)
                return Predicate == null ? $"Get({Table.Name} AS {Alias})" : $"Get({Table.Name} AS {Alias} WHERE {Predicate})";
            return Predicate == null ? $"Get({Table.Name})" : $"Get({Table.Name} WHERE {Predicate})";
        }
    }
    
    // TODO join types
    public record Join(Node Left, Node Right, AlgebraExpression? Predicate) : LogicalNode
    {
        public override string ToString() => Predicate != null ? $"Join({Left}, {Right}, {Predicate})" : $"Join({Left}, {Right})";
    }

    public record Projection(Node Inner, IReadOnlyCollection<AlgebraExpression> Columns) : LogicalNode
    {
        public override string ToString() => $"Projection({Inner}, {string.Join(", ", Columns)})";
    }

    public record Filter(Node Inner, AlgebraExpression Predicate) : LogicalNode
    {
        public override string ToString() => $"Filter({Inner}, {Predicate})";
    }
}