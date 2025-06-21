using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SimpleDatabase.Parsing.Expressions;
using SimpleDatabase.Parsing.Statements;
using SimpleDatabase.Parsing.Tables;
using SimpleDatabase.Planning.Queries;

namespace SimpleDatabase.Planning.Optimisation.Algebra;

public class QueryToAlgebra
{
    public static Node Build(ResolvedQuery context)
    {
        var query = context.Query;
        
        var node = BuildFrom(context, query.From.Table);
        
       foreach (var tableJoin in query.From.Joins)
       {
           var inner = BuildFrom(context, tableJoin.Table);
           var predicate = BuildExpression(tableJoin.Predicate);
           
           node = new LogicalNode.Join(node, inner, predicate);
       }
        
        if (query.Where.HasValue)
        {
            var predicate = BuildExpression(query.Where.Value!);
            
            node = new LogicalNode.Filter(node, predicate);
        }

        if (query.Ordering.Any())
        {
            throw new NotImplementedException();
        }

        // TODO handle naming
        // TODO var columns = query.Columns.Cast<ResultColumn.Expression>().Select(x => BuildExpression(x.Value)).ToList();
        // TODO node = new LogicalNode.Projection(node, columns);
        
        return node;
    }
    
    private static Node BuildFrom(ResolvedQuery context, TableAlias from)
    {
        var (alias, table) = context.Tables.GetByAlias(from.Alias);
        return new LogicalNode.Get(alias, table);
    }
    
    [return: NotNullIfNotNull(nameof(expression))]
    private static AlgebraExpression? BuildExpression(Expression? expression)
    {
        if (expression == null)
        {
            return null;
        }

        return expression switch
        {
            BinaryExpression { Operator: BinaryOperator.Equal } binary => new AlgebraExpression.Equality(BuildExpression(binary.Left), BuildExpression(binary.Right)),
            ColumnNameExpression columnName => new AlgebraExpression.Column(columnName.Table!, columnName.Name),
            
            NumberLiteralExpression literal => new AlgebraExpression.NumberLiteral(literal.Value),
            
            _ => throw new ArgumentOutOfRangeException(nameof(expression), $"unhandled expression type: {expression.GetType().Name}")
        };
    }
}