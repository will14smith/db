using System;
using System.Collections.Generic;
using System.Linq;
using SimpleDatabase.Parsing.Antlr;
using SimpleDatabase.Parsing.Statements;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Parsing.Visitors
{
    internal class StatementDataDefinitionVisitor : SQLBaseVisitor<StatementDataDefinition>
    {
        public override StatementDataDefinition VisitStatement_ddl_create_table(SQLParser.Statement_ddl_create_tableContext context)
        {
            var tableName = context.Table.IDENTIFIER().GetText();
            var existsCheck = context.K_EXISTS() != null;
            var columns = context._Columns.Select(HandleColumnDefinition).ToList();
            
            return new CreateTableStatement(tableName, existsCheck, columns);
        }

        public override StatementDataDefinition VisitStatement_ddl_create_index(SQLParser.Statement_ddl_create_indexContext context)
        {
            var indexName = context.Index.IDENTIFIER().GetText();
            var tableName = context.Table.IDENTIFIER().GetText();
            var existsCheck = context.K_EXISTS() != null;
            var keys = context._Columns.Select(HandleIndexColumnDefinition).ToList();
            
            var includingContext = context.statement_ddl_create_index_including();
            IReadOnlyList<string> including = includingContext == null ? Array.Empty<string>() : includingContext._Columns.Select(x => x.IDENTIFIER().GetText()).ToList();
            
            return new CreateIndexStatement(indexName, tableName, existsCheck, keys, including);
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
        
        private static IndexColumnDefinition HandleIndexColumnDefinition(SQLParser.Index_columnContext context)
        {
            var name = context.column_name().IDENTIFIER().GetText();
            var order = context.K_DESC() == null ? Order.Ascending : Order.Descending;

            return new IndexColumnDefinition(name, order);
        }
    }
}