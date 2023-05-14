using System.Collections.Generic;
using SimpleDatabase.Parsing.Expressions;
using SimpleDatabase.Parsing.Statements;
using SimpleDatabase.Schemas;

namespace SimpleDatabase.Planning;

public static class IndexSelector
{
    public static bool IsUsable(TableIndex index, Expression? predicate, IReadOnlyList<OrderExpression> ordering)
    {
        // TODO handle multi-column indexes
        var (keyColumn, keyOrdering) = index.Structure.Keys[0];
        
        // TODO handle more complex expressions
        while (predicate is BinaryExpression { Operator: BinaryOperator.BooleanAnd, Left: var andLeft })
        {
            predicate = andLeft;
        }
        
        if (predicate is BinaryExpression { Operator: BinaryOperator.Equal, Left: ColumnNameExpression equalityColumnName, Right: LiteralExpression } && equalityColumnName.Name == keyColumn.Name)
        {
            return true;
        }

        // TODO handle multi-column ordering
        if (ordering.Count > 0 && ordering[0].Expression is ColumnNameExpression orderingColumn && orderingColumn.Name == keyColumn.Name & Equal(ordering[0].Order, keyOrdering))
        {
            return true;
        }

        return false;
    }

    private static bool Equal(Order columnOrdering, KeyOrdering keyOrdering)
    {
        switch (columnOrdering)
        {
            case Order.Ascending when keyOrdering == KeyOrdering.Ascending:
            case Order.Descending when keyOrdering == KeyOrdering.Descending:
                return true;
            
            default: return false;
        }
    }
}