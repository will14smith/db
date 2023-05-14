using System.Collections.Generic;
using SimpleDatabase.Parsing.Expressions;

namespace SimpleDatabase.Planning.Nodes
{
    public class ConstantNode : Node
    {
        public IReadOnlyList<string> Columns { get; }
        public IReadOnlyList<IReadOnlyList<Expression>> Values { get; }

        public ConstantNode(string alias, IReadOnlyList<string> columns, IReadOnlyList<IReadOnlyList<Expression>> values) : base(alias)
        {
            Columns = columns;
            Values = values;
        }
    }
}
