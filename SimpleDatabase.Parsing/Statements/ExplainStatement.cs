using System;

namespace SimpleDatabase.Parsing.Statements;

public class ExplainStatement : StatementDataManipulation
{
    public StatementDataManipulation Statement { get; }

    public ExplainStatement(StatementDataManipulation statement)
    {
        if (statement is ExplainStatement)
        {
            throw new InvalidOperationException("Cannot EXPLAIN the EXPLAINation...");
        }

        Statement = statement;
    }
}