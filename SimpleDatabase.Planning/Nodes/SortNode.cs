﻿using System.Collections.Generic;
using SimpleDatabase.Parsing.Statements;

namespace SimpleDatabase.Planning.Nodes
{
    public class SortNode : Node
    {
        public Node Input { get; }
        public IEnumerable<OrderExpression> Orderings { get; }

        public SortNode(Node input, IEnumerable<OrderExpression> orderings)
        {
            Input = input;
            Orderings = orderings;
        }
    }
}
