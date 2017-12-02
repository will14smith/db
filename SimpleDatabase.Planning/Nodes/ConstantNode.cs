using System;
using System.Collections.Generic;
using System.Text;
using SimpleDatabase.Parsing.Expressions;

namespace SimpleDatabase.Planning.Nodes
{
    public class ConstantNode : Node
    {
        public IReadOnlyList<string> Columns { get; }
        public IReadOnlyList<IReadOnlyList<Expression>> Values { get; }

        public ConstantNode(IReadOnlyList<string> columns, IReadOnlyList<IReadOnlyList<Expression>> values)
        {
            Columns = columns;
            Values = values;
        }
    }
}
