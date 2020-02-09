using System.Collections.Generic;
using SimpleDatabase.Parsing.Antlr;
using SimpleDatabase.Parsing.Statements;

namespace SimpleDatabase.Parsing.Visitors
{
    internal class ProgramVisitor : SQLBaseVisitor<IReadOnlyList<Statement>>
    {
        public override IReadOnlyList<Statement> VisitProgram(SQLParser.ProgramContext context)
        {
            var statements = new List<Statement>();

            foreach(var statementTree in context.statement())
            {
                var statement = statementTree.Accept(new StatementVisitor());
                statements.Add(statement);
            }

            return statements;
        }
    }
}