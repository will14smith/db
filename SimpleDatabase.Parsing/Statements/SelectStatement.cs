using System;
using System.Collections.Generic;
using SimpleDatabase.Parsing.Expressions;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Parsing.Statements;

public class SelectStatement : StatementDataManipulation
{
    public IReadOnlyList<ResultColumn> Columns { get; }
    public TableRef Table { get; }
    public Option<Expression> Where { get; }
    public IReadOnlyList<OrderExpression> Ordering { get; }

    public SelectStatement(IReadOnlyList<ResultColumn> columns, TableRef table, Option<Expression> where, IReadOnlyList<OrderExpression> ordering)
    {
        Columns = columns;
        Table = table;
        Where = where;
        Ordering = ordering;
    }
}

public class OrderExpression : IEquatable<OrderExpression>
{
    public Expression Expression { get; }
    public Order Order { get; }

    public OrderExpression(Expression expression, Order order)
    {
        Expression = expression;
        Order = order;
    }

    public bool Equals(OrderExpression? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Expression.Equals(other.Expression) && Order == other.Order;
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is OrderExpression other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (Expression.GetHashCode() * 397) ^ (int) Order;
        }
    }

    public static bool operator ==(OrderExpression? left, OrderExpression? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(OrderExpression? left, OrderExpression? right)
    {
        return !Equals(left, right);
    }
}

public enum Order
{
    Ascending,
    Descending,
}