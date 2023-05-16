using System;
using System.Collections.Generic;
using System.Linq;
using SimpleDatabase.Parsing.Expressions;
using SimpleDatabase.Parsing.Statements;
using SimpleDatabase.Planning.Nodes;
using SimpleDatabase.Schemas;

namespace SimpleDatabase.Planning;

public class PlanBuilder
{
    public static IEnumerable<Plan> EnumeratePlans(Table table, IReadOnlyList<ResultColumn> columns, Expression? predicate, IReadOnlyList<OrderExpression> ordering)
    {
        var terms = predicate == null? new List<Expression>() : PredicateToTerms(predicate);
        var fields = RequiredFields(table, columns, predicate, ordering).ToHashSet();
        
        foreach (var tableIndex in table.Indexes)
        {
            foreach (var (key, order) in tableIndex.Structure.Keys)
            {
                var matchingTerm = terms.FirstOrDefault(t => t is BinaryExpression { Operator: BinaryOperator.Equal, Left: ColumnNameExpression termBinaryEqualColumnName } && termBinaryEqualColumnName.Name == key.Name);
                if (matchingTerm == null) break;

                var indexFields = FieldsProvidedByIndex(tableIndex);
                
                var remainingTerms = terms.Except(new [] { matchingTerm }).ToList();
                var remainingColumns = fields.Except(indexFields).ToList();
                // TODO support ordering by index
                var remainingOrdering = ordering.ToList();
                
                var aliasCounter = 0;
                
                // TODO are any other terms satisfied by this index?
                var seekNode = new SeekIndexNode($"i{aliasCounter++}", table.Name, tableIndex, matchingTerm);
                Node node = seekNode;
                
                if (remainingColumns.Count > 0)
                {
                    var rowIdLookupNode = new RowIdLookupNode($"t{aliasCounter++}", table.Name, new NodeOutputExpression(seekNode.Alias, 0));
                    var projectionNode = new ProjectionNode($"p{aliasCounter++}", rowIdLookupNode, remainingColumns.Select(x => new ResultColumn.Expression(new ColumnNameExpression(x), null)).ToList());
                    
                    // TODO remainingTerms in this predicate?
                    var joinNode = new NestedLoopJoinNode($"j{aliasCounter++}", seekNode, projectionNode, null);

                    node = joinNode;
                }

                if (remainingTerms.Count > 0)
                {
                    var remainingPredicate = remainingTerms.Aggregate((a, b) => new BinaryExpression(BinaryOperator.BooleanAnd, a, b));
                    node = new FilterNode($"j{aliasCounter++}", node, remainingPredicate);
                }

                if (remainingOrdering.Count > 0)
                {
                    node = new SortNode($"s{aliasCounter++}", node, remainingOrdering);
                }

                if (columns is not [ResultColumn.Star { Table: null }])
                {
                    node = new ProjectionNode($"p{aliasCounter++}", node, columns);
                }

                yield return new Plan(node);
            }
        }

        if (ordering.Count == 1 && ordering[0].Expression is ColumnNameExpression orderingColumn)
        {
            foreach (var tableIndex in table.Indexes)
            {
                foreach (var (key, order) in tableIndex.Structure.Keys)
                {
                    if (key.Name != orderingColumn.Name || !SameOrder(order, ordering[0].Order)) break;

                    var indexFields = FieldsProvidedByIndex(tableIndex);

                    var remainingColumns = fields.Except(indexFields).ToList();

                    var aliasCounter = 0;

                    // TODO are any other terms satisfied by this index?
                    var scanNode = new ScanIndexNode($"i{aliasCounter++}", table.Name, tableIndex.Name);
                    Node node = scanNode;
                    
                    if (remainingColumns.Count > 0)
                    {
                        var rowIdLookupNode = new RowIdLookupNode($"t{aliasCounter++}", table.Name, new NodeOutputExpression(scanNode.Alias, 0));
                        var projectionNode = new ProjectionNode($"p{aliasCounter++}", rowIdLookupNode, remainingColumns.Select(x => new ResultColumn.Expression(new ColumnNameExpression(x), null)).ToList());

                        // TODO remainingTerms in this predicate?
                        var joinNode = new NestedLoopJoinNode($"j{aliasCounter++}", scanNode, projectionNode, null);

                        node = joinNode;
                    }

                    if (terms.Count > 0)
                    {
                        var remainingPredicate = terms.Aggregate((a, b) => new BinaryExpression(BinaryOperator.BooleanAnd, a, b));
                        node = new FilterNode($"f{aliasCounter++}", node, remainingPredicate);
                    }
                    
                    if (columns is not [ResultColumn.Star { Table: null }])
                    {
                        node = new ProjectionNode($"p{aliasCounter++}", node, columns);
                    }

                    yield return new Plan(node);
                }
            }
        }

        yield return TableScan(table, columns, predicate, ordering);
    }
    
    private static Plan TableScan(Table table, IReadOnlyList<ResultColumn> columns, Expression? predicate, IReadOnlyList<OrderExpression> ordering)
    {
        var aliasCounter = 0;
        
        Node node = new ScanTableNode($"t{aliasCounter++}", table.Name);
        
        // filter
        if (predicate != null)
        {
            node = new FilterNode($"f{aliasCounter++}", node, predicate);
        }
        
        // sort
        if (ordering.Any())
        {
            node = new SortNode($"s{aliasCounter++}", node, ordering);
        }
        
        // project
        if (columns is not [ResultColumn.Star { Table: null }])
        {
            node = new ProjectionNode($"p{aliasCounter++}", node, columns);
        }

        return new Plan(node);
    }

    private static List<Expression> PredicateToTerms(Expression expression)
    {
        var terms = new List<Expression>();
        PredicateToTerms(expression, terms);
        return terms;
    }
    private static void PredicateToTerms(Expression expression, List<Expression> terms)
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
    
    private static IEnumerable<string> RequiredFields(Table table, IEnumerable<ResultColumn> columns, Expression? predicate, IReadOnlyCollection<OrderExpression> ordering)
    {
        var fields = new HashSet<string>();

        foreach (var column in columns)
        {
            switch (column)
            {
                case ResultColumn.Expression expression:
                    fields.UnionWith(RequiredFields(expression.Value));
                    break;
                    
                case ResultColumn.Star:
                    fields.UnionWith(table.Columns.Select(x => x.Name));
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(column));
            }
        }

        if (predicate != null)
        {
            fields.UnionWith(RequiredFields(predicate));
        }
        
        foreach (var order in ordering)
        {
            fields.UnionWith(RequiredFields(order.Expression));
        }
        
        return fields;
    }

    private static IEnumerable<string> RequiredFields(Expression predicate)
    {
        switch (predicate)
        {
            case BinaryExpression binaryExpression:
                foreach (var field in RequiredFields(binaryExpression.Left)) yield return field;
                foreach (var field in RequiredFields(binaryExpression.Right)) yield return field;
                break;
            case ColumnNameExpression columnNameExpression:
                yield return columnNameExpression.Name;
                break;

            case LiteralExpression:
            case NodeOutputExpression:
                yield break;
            
            default: throw new ArgumentOutOfRangeException(nameof(predicate));
        }
    }

    private static IEnumerable<string> FieldsProvidedByIndex(TableIndex tableIndex)
    {
        return tableIndex.Structure.Keys.Select(x => x.Item1.Name)
            .Concat(tableIndex.Structure.Data.Select(x => x.Name));
    }
    
    private static bool SameOrder(KeyOrdering order, Order order1)
    {
        if (order == KeyOrdering.Ascending) return order1 == Order.Ascending;
        return order1 == Order.Descending;
    }
}

public partial class Planner
{
    private Plan PlanSelect(SelectStatement select)
    {
        var tableName = ((TableRef.TableName)select.Table).Name;
        var table = _database.GetTable(tableName);

        return PlanBuilder.EnumeratePlans(table, select.Columns, select.Where.Value, select.Ordering).First();
    }
}