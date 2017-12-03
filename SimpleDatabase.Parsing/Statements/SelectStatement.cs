using System.Collections.Generic;
using SimpleDatabase.Parsing.Expressions;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Parsing.Statements
{
    public class SelectStatement : Statement
    {
        public IReadOnlyList<ResultColumn> Columns { get; }
        public Table Table { get; }
        public Option<Expression> Where { get; }
        public IReadOnlyList<OrderExpression> Ordering { get; }

        public SelectStatement(IReadOnlyList<ResultColumn> columns, Table table, Option<Expression> where, IReadOnlyList<OrderExpression> ordering)
        {
            Columns = columns;
            Table = table;
            Where = where;
            Ordering = ordering;
        }
    }

    public class OrderExpression
    {
        public Expression Expression { get; }
        public Order Order { get; }

        public OrderExpression(Expression expression, Order order)
        {
            Expression = expression;
            Order = order;
        }
    }

    public enum Order
    {
        Ascending,
        Descending,
    }
}