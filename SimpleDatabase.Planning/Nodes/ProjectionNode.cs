using System.Collections.Generic;
using SimpleDatabase.Parsing.Statements;

namespace SimpleDatabase.Planning.Nodes
{
    public class ProjectionNode : Node
    {
        public Node Input { get; }
        public IReadOnlyList<ResultColumn> Columns { get; }

        public ProjectionNode(Node input, IReadOnlyList<ResultColumn> columns)
        {
            Input = input;
            Columns = columns;
        }
    }
}