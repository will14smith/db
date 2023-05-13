using System;
using System.Linq;
using SimpleDatabase.Parsing.Antlr;
using SimpleDatabase.Parsing.Statements;

namespace SimpleDatabase.Parsing.Visitors
{
    internal class StatementDataDefinitionVisitor : SQLBaseVisitor<StatementDataDefinition>
    {
        public override StatementDataDefinition VisitStatement_ddl_create_table(SQLParser.Statement_ddl_create_tableContext context)
        {
            var tableName = context.Table.IDENTIFIER().GetText();
            var existsCheck = context.K_EXISTS() != null;
            var columns = context.column_definition_list()._Columns.Select(HandleColumnDefinition).ToList();
            
            return new CreateTableStatement(tableName, existsCheck, columns);
        }

        private static ColumnDefinition HandleColumnDefinition(SQLParser.Column_definitionContext context)
        {
            var name = context.column_name().IDENTIFIER().GetText();
            var type = HandleColumnDefinitionType(context.column_type());
            
            return new ColumnDefinition(name, type);
        }

        private static ColumnDefinitionType HandleColumnDefinitionType(SQLParser.Column_typeContext typeContext)
        {
            return typeContext switch
            {
                SQLParser.Column_type_no_argsContext context => new ColumnDefinitionType(context.IDENTIFIER().GetText(), Array.Empty<int>()),
                SQLParser.Column_type_one_argContext context => new ColumnDefinitionType(context.IDENTIFIER().GetText(), new [] { int.Parse(context.NUMBER_LITERAL().GetText()) }),

                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}