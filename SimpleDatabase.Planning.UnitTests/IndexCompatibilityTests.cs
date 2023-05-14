using System;
using System.Collections.Generic;
using System.Linq;
using SimpleDatabase.Parsing.Expressions;
using SimpleDatabase.Parsing.Statements;
using SimpleDatabase.Schemas;
using SimpleDatabase.Schemas.Types;
using Xunit;

namespace SimpleDatabase.Planning.UnitTests;

public class IndexCompatibilityTests
{
    public static IEnumerable<object?[]> SingleColumnIndexCases
    {
        get
        {
            yield return PredicateT(Equal("name", "Will"));
            yield return PredicateF(Equal("email", "Will"));
            yield return PredicateT(And(Equal("name", "Will"), Equal("country", "GB")));
            yield return PredicateF(And(Equal("id", 1), Equal("country", "Will")));
            // TODO this case is actually possible but more complex so just saying unusable for now
            yield return PredicateF(Or(Equal("name", "Will"), Equal("country", "GB")));
            
            yield return OrderT(("name", Order.Ascending));
            yield return OrderF(("name", Order.Descending));
            yield return OrderT(("name", Order.Ascending), ("country", Order.Ascending));
            yield return OrderF(("country", Order.Ascending));
        }
    }

    public static IEnumerable<object?[]> MultipleColumnIndexCases
    {
        get
        {
            yield return PredicateT(Equal("name", "Will"));
            yield return PredicateF(Equal("country", "GB"));
            yield return PredicateOrderT(Equal("name", "Will"), ("country", Order.Ascending));
            yield return OrderT(("name", Order.Ascending));
            yield return OrderF(("country", Order.Ascending));
        }
    }

    [Theory, MemberData(nameof(SingleColumnIndexCases))]
    public void SingleColumnIndex(Expression? predicate, IReadOnlyList<OrderExpression> ordering, bool expected)
    {
        var index = new TableIndex("ix_name", new KeyStructure(new[]
        {
            (new Column("name", new ColumnType.String(1)), KeyOrdering.Ascending),
        }, Array.Empty<Column>()));
        
        var result = IndexSelector.IsUsable(index, predicate, ordering);

        Assert.Equal(expected, result);
    }

    [Theory, MemberData(nameof(MultipleColumnIndexCases))]
    public void MultipleColumnIndex(Expression? predicate, IReadOnlyList<OrderExpression> ordering, bool expected)
    {
        var index = new TableIndex("ix_name_country", new KeyStructure(new[]
        {
            (new Column("name", new ColumnType.String(1)), KeyOrdering.Ascending),
            (new Column("country", new ColumnType.String(1)), KeyOrdering.Ascending),
        }, Array.Empty<Column>()));
        
        var result = IndexSelector.IsUsable(index, predicate, ordering);

        Assert.Equal(expected, result);
    }
    
    private static object?[] PredicateT(Expression expression) => new object[] { expression, Array.Empty<OrderExpression>(), true };
    private static object?[] PredicateF(Expression expression) => new object[] { expression, Array.Empty<OrderExpression>(), false };
    private static object?[] OrderT(params (string, Order)[] ordering) => new object?[] { null, ordering.Select(x => new OrderExpression(new ColumnNameExpression(x.Item1), x.Item2)).ToList(), true };
    private static object?[] OrderF(params (string, Order)[] ordering) => new object?[] { null, ordering.Select(x => new OrderExpression(new ColumnNameExpression(x.Item1), x.Item2)).ToList(), false };
    private static object?[] PredicateOrderT(Expression expression, params (string, Order)[] ordering) => new object?[] { expression, ordering.Select(x => new OrderExpression(new ColumnNameExpression(x.Item1), x.Item2)).ToList(), true };
    private static object?[] PredicateOrderF(Expression expression, params (string, Order)[] ordering) => new object?[] { expression, ordering.Select(x => new OrderExpression(new ColumnNameExpression(x.Item1), x.Item2)).ToList(), false };
    
    private static Expression Equal(string column, string value) => new BinaryExpression(BinaryOperator.Equal, new ColumnNameExpression(column), new StringLiteralExpression(value));
    private static Expression Equal(string column, int value) => new BinaryExpression(BinaryOperator.Equal, new ColumnNameExpression(column), new NumberLiteralExpression(value));
    private static Expression And(Expression lhs, Expression rhs) => new BinaryExpression(BinaryOperator.BooleanAnd, lhs, rhs);
    private static Expression Or(Expression lhs, Expression rhs) => new BinaryExpression(BinaryOperator.BooleanOr, lhs, rhs);
}