using System.Collections.Generic;
using SimpleDatabase.Parsing.Statements;

namespace SimpleDatabase.Planning.Nodes
{
    public class SortNode : Node
    {
        public Node Input { get; }
        public IReadOnlyList<OrderExpression> Orderings { get; }

        public SortNode(string alias, Node input, IReadOnlyList<OrderExpression> orderings) : base(alias)
        {
            Input = input;
            Orderings = orderings;
        }
    }
}
