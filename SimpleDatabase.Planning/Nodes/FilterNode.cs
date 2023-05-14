using SimpleDatabase.Parsing.Expressions;

namespace SimpleDatabase.Planning.Nodes
{
    public class FilterNode : Node
    {
        public Node Input { get; }
        public Expression Predicate { get; }

        public FilterNode(string alias, Node input, Expression predicate) : base(alias)
        {
            Input = input;
            Predicate = predicate;
        }
    }
}