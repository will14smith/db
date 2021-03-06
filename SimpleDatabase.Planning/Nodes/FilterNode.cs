﻿using SimpleDatabase.Parsing.Expressions;

namespace SimpleDatabase.Planning.Nodes
{
    public class FilterNode : Node
    {
        public Node Input { get; }
        public Expression Predicate { get; }

        public FilterNode(Node input, Expression predicate)
        {
            Input = input;
            Predicate = predicate;
        }
    }
}