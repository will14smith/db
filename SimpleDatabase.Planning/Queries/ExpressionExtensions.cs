using System;
using System.Collections.Generic;
using System.Linq;
using SimpleDatabase.Parsing.Expressions;
using SimpleDatabase.Planning.Nodes;

namespace SimpleDatabase.Planning.Queries;

public static class ExpressionExtensions
{
    public static IReadOnlyList<Expression> PredicateToTerms(Expression? expression)
    {
        var terms = new List<Expression>();
        if (expression != null)
        {
            PredicateToTerms(expression, terms);
        }
        return terms;
    }
    public static void PredicateToTerms(Expression expression, List<Expression> terms)
    {
        if (expression is BinaryExpression { Operator: BinaryOperator.BooleanAnd, Left: var andLhs, Right: var andRhs })
        {
            PredicateToTerms(andLhs, terms);
            PredicateToTerms(andRhs, terms);
        }
        else
        {
            terms.Add(expression);
        }
    }

    public static IReadOnlyDictionary<string, IReadOnlyList<Expression>> GroupTermsByTables(IEnumerable<Expression> terms) =>
        terms
            .SelectMany(ReferencedTables, (term, table) => (Table: table, Term: term))
            .GroupBy(x => x.Table, x => x.Term)
            .ToDictionary(x => x.Key, x => (IReadOnlyList<Expression>) x.ToList());

    public static IReadOnlyCollection<ColumnNameExpression> ReferencedColumns(Expression expression)
    {
        var columns = new HashSet<ColumnNameExpression>();
        Visit(expression);
        return columns;

        void Visit(Expression exprInner)
        {
            switch (exprInner)
            {
                case BinaryExpression binary:
                    Visit(binary.Left);
                    Visit(binary.Right);
                    break;
                
                case ColumnNameExpression columnName: 
                    columns.Add(columnName);
                    break;
                
                case LiteralExpression: break;
                case NodeOutputExpression: break;
                
                default: throw new ArgumentOutOfRangeException(nameof(exprInner));
            }
        }
    }
    
    public static IReadOnlyCollection<string> ReferencedTables(Expression expression) => 
        ReferencedColumns(expression)
            .Select(x => x.Table ?? throw new Exception("expression must be fully resolved before find referenced tables"))
            .ToHashSet();
}