using System;

namespace SimpleDatabase.Parsing.Statements
{
    public class ExplainStatement : Statement
    {
        public Statement Statement { get; }

        public ExplainStatement(Statement statement)
        {
            if (statement is ExplainStatement)
            {
                throw new InvalidOperationException("Cannot EXPLAIN the EXPLAINation...");
            }

            Statement = statement;
        }
    }
}