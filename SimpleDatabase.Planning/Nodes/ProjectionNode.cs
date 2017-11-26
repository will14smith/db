using System.Collections.Generic;
using SimpleDatabase.Parsing.Statements;

namespace SimpleDatabase.Planning.Nodes
{
    public class ProjectionNode : Node
    {
        public Node Input { get; }
        public List<ResultColumn> Columns { get; }

        public ProjectionNode(Node input, List<ResultColumn> columns)
        {
            Input = input;
            Columns = columns;
        }
    }
}