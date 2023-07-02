using System;
using System.Collections.Generic;
using System.Linq;
using SimpleDatabase.Parsing.Antlr;
using SimpleDatabase.Parsing.Expressions;
using SimpleDatabase.Parsing.Statements;
using SimpleDatabase.Parsing.Tables;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Parsing.Visitors
{
    internal class StatementDataManipulationVisitor : SQLBaseVisitor<StatementDataManipulation>
    {
        public override StatementDataManipulation VisitStatement_dml(SQLParser.Statement_dmlContext context)
        {
            var statement = context.GetChild(context.ChildCount - 1).Accept(this);
        
            if (context.K_EXPLAIN() != null)
            {
                return new ExplainStatement(statement, context.K_EXECUTE() != null);
            }
        
            return statement;
        }

        public override StatementDataManipulation VisitStatement_dml_select(SQLParser.Statement_dml_selectContext context)
        {
            var columns = context._Columns.Select(HandleResultColumn).ToList();
            var from = HandleFrom(context.table_from());
            var where = context.Where.ToOption().Select(HandleExpression);
            var ordering = context._Ordering.Select(HandleOrdering).ToList();
        
            return new SelectStatement(columns, from, where, ordering);
        
            OrderExpression HandleOrdering(SQLParser.Ordering_termContext arg)
            {
                var expr = HandleExpression(arg.expression());
                var order = arg.K_DESC() == null ? Order.Ascending : Order.Descending;
        
                return new OrderExpression(expr, order);
            }
        }
        
        public override StatementDataManipulation VisitStatement_dml_insert(SQLParser.Statement_dml_insertContext context)
        {
            var table = context.Table.IDENTIFIER().GetText();
            var columns = context._Columns.Select(x => x.IDENTIFIER().GetText()).ToList();
        
            var values = context._Values.Select(HandleValues).ToList();
        
            return new InsertStatement(table, columns, values);
        
            IReadOnlyList<Expression> HandleValues(SQLParser.Statement_dml_insert_valueContext arg)
            {
                return arg._Values.Select(HandleExpression).ToList();
            }
        }
        
        public override StatementDataManipulation VisitStatement_dml_delete(SQLParser.Statement_dml_deleteContext context)
        {
            var table = context.Table.IDENTIFIER().GetText();
            var where = context.Where.ToOption().Select(HandleExpression);
        
            return new DeleteStatement(table, where);
        }
        
        private static Expression HandleExpression(SQLParser.ExpressionContext context)
        {
            return context.Accept(new ExpressionVisitor());
        }
        
        private static ResultColumn HandleResultColumn(SQLParser.Result_columnContext context)
        {
            switch (context)
            {
                case SQLParser.Result_column_starContext star:
                    {
                        var table = star.table_name().Select(x => x.IDENTIFIER().GetText());
        
                        return new ResultColumn.Star(table);
                    }
                case SQLParser.Result_column_exprContext expression:
                    {
                        var parsedExpression = HandleExpression(expression.expression());
                        var alias = expression.column_alias().Select(x => x.IDENTIFIER().GetText());
        
                        return new ResultColumn.Expression(parsedExpression, alias);
                    }
                default:
                    throw new NotImplementedException(context.GetType().FullName);
            }
        }
        
        private static TableFrom HandleFrom(SQLParser.Table_fromContext from)
        {
            var table = HandleTableAlias(from.table_alias());

            var joinContexts = from.table_join();
            if (joinContexts.Length == 0)
            {
                return new TableFrom(table);
            }

            var joins = new List<TableJoin>();

            foreach (var joinContext in joinContexts)
            {
                var joinTable = HandleTableAlias(joinContext.table_alias());
                var joinPredicate = joinContext.expression() != null ? HandleExpression(joinContext.expression()) : null;
                
                var join = new TableJoin(joinTable, joinPredicate);
                
                joins.Add(join);
            }
            
            return new TableFrom(table, joins);
        }
        
        private static TableAlias HandleTableAlias(SQLParser.Table_aliasContext context)
        {
            var name = context.table.IDENTIFIER().GetText();
            var alias = context.alias?.IDENTIFIER().GetText() ?? name;

            return new TableAlias(name, alias);
        }
    }
}