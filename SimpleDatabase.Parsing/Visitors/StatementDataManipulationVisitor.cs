using System;
using System.Collections.Generic;
using System.Linq;
using SimpleDatabase.Parsing.Antlr;
using SimpleDatabase.Parsing.Expressions;
using SimpleDatabase.Parsing.Statements;
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
                return new ExplainStatement(statement);
            }
        
            return statement;
        }

        public override StatementDataManipulation VisitStatement_dml_select(SQLParser.Statement_dml_selectContext context)
        {
            var columns = context._Columns.Select(HandleResultColumn).ToList();
            var table = HandleTable(context.Table);
            var where = context.Where.ToOption().Select(HandleExpression);
            var ordering = context._Ordering.Select(HandleOrdering).ToList();
        
            return new SelectStatement(columns, table, where, ordering);
        
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
        
        private static Table HandleTable(SQLParser.Table_nameContext context)
        {
            return new Table.TableName(context.IDENTIFIER().GetText());
        }
    }
}