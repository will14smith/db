using SimpleDatabase.Schemas;

namespace SimpleDatabase.Planning.Optimisation.Algebra;

public abstract record PhysicalNode : Node
{
    public record TableScan(Table Table, AlgebraExpression? Predicate) : PhysicalNode;
    public record IndexScan(Table Table, TableIndex Index) : PhysicalNode;
    public record InnerNestedLoopJoin(Node Outer, Node Inner, AlgebraExpression Predicate) : PhysicalNode;
}