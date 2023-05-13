using SimpleDatabase.Parsing.Antlr;
using SimpleDatabase.Parsing.Statements;

namespace SimpleDatabase.Parsing.Visitors
{
    internal class StatementVisitor : SQLBaseVisitor<Statement>
    {
        public override Statement VisitStatement_ddl(SQLParser.Statement_ddlContext context) => context.Accept(new StatementDataDefinitionVisitor());
        public override Statement VisitStatement_dml(SQLParser.Statement_dmlContext context) => context.Accept(new StatementDataManipulationVisitor());
    }
}