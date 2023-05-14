using System.Collections.Generic;
using SimpleDatabase.Parsing.Statements;

namespace SimpleDatabase.Planning.Nodes
{
    public class ProjectionNode : Node
    {
        public Node Input { get; }
        public IReadOnlyList<ResultColumn> Columns { get; }

        public ProjectionNode(string alias, Node input, IReadOnlyList<ResultColumn> columns) : base(alias)
        {
            Input = input;
            Columns = columns;
        }
    }
}