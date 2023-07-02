using System;
using System.Collections.Generic;
using System.Linq;
using SimpleDatabase.Parsing.Expressions;
using SimpleDatabase.Parsing.Statements;
using SimpleDatabase.Parsing.Tables;
using SimpleDatabase.Planning.Nodes;
using SimpleDatabase.Schemas;

namespace SimpleDatabase.Planning;

public static class PlanBuilder
{
    public static IEnumerable<Plan> EnumeratePlans(Table table, IReadOnlyList<ResultColumn> columns, Expression? predicate, IReadOnlyList<OrderExpression> ordering)
    {
        var terms = predicate == null? new List<Expression>() : PredicateToTerms(predicate);
        var projectionFields = ProjectionFields(table, columns).ToHashSet();
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

                node = new ProjectionNode($"p{aliasCounter++}", node, projectionFields.Select(x => new ResultColumn.Expression(new ColumnNameExpression(x), x)).ToList());
                
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
                    
                    node = new ProjectionNode($"p{aliasCounter++}", node, projectionFields.Select(x => new ResultColumn.Expression(new ColumnNameExpression(x), x)).ToList());

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
        var fields = ProjectionFields(table, columns).ToHashSet();

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

    private static IEnumerable<string> ProjectionFields(Table table, IEnumerable<ResultColumn> columns)
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
        var tables = GetTables(select.From);
        var columns = ResolveResultColumns(select.Columns, tables);
        var predicate = select.Where.Value;
        
        if (select.From.Joins.Any())
        {
            predicate = select.From.Joins
                .Select(join => join.Predicate)
                .Aggregate(predicate, (current, joinPredicate) => joinPredicate == null ? current : current == null ? joinPredicate : new BinaryExpression(BinaryOperator.BooleanAnd, current, joinPredicate));
        }

        predicate = predicate == null ? null : ResolveExpression(predicate, tables);
        
        var tableName = select.From.Table.Name;
        var table = _database.GetTable(tableName);
        
        return PlanBuilder.EnumeratePlans(table, columns, predicate, select.Ordering).First();
    }

    private IReadOnlyDictionary<string, Table> GetTables(TableFrom from)
    {
        var tables = new Dictionary<string, Table>
        {
            { from.Table.Alias, _database.GetTable(from.Table.Name) }
        };

        foreach (var join in from.Joins)
        {
            tables.Add(join.Table.Alias, _database.GetTable(join.Table.Name));
        }
        
        return tables;
    }

    private static IReadOnlyList<ResultColumn.Expression> ResolveResultColumns(IReadOnlyList<ResultColumn> columns, IReadOnlyDictionary<string, Table> tables)
    {
        var resolved = new List<ResultColumn.Expression>();

        foreach (var column in columns)
        {
            switch (column)
            {
                case ResultColumn.Star star:
                    var (tableAlias, table) = ResolveTable(tables, star.Table);
                    resolved.AddRange(table.Columns.Select(tableColumn => new ResultColumn.Expression(new ColumnNameExpression(tableAlias, tableColumn.Name), tableColumn.Name)));
                    break;

                case ResultColumn.Expression expression: resolved.Add(new ResultColumn.Expression(ResolveExpression(expression.Value, tables), expression.Alias)); break;
   
                default: throw new ArgumentOutOfRangeException(nameof(column));
            }
        }
        
        return resolved;
    }
    
    private static (string Alias, Table Table) ResolveTable(IReadOnlyDictionary<string, Table> tables, string? alias)
    {
        if (alias != null)
        {
            return (alias, tables[alias]);
        }

        if (tables.Count != 1)
        {
            throw new Exception("ambiguous star in result columns");
        }
        
        var (tableAlias, table) = tables.First();
        return (tableAlias, table);
    }
    
    private static Expression ResolveExpression(Expression expression, IReadOnlyDictionary<string, Table> tables)
    {
        return expression switch
        {
            BinaryExpression binaryExpression => new BinaryExpression(
                binaryExpression.Operator,
                ResolveExpression(binaryExpression.Left, tables),
                ResolveExpression(binaryExpression.Right, tables)),
            
            ColumnNameExpression { Table: not null } => expression,
            ColumnNameExpression columnNameExpression => new ColumnNameExpression(ResolveColumn(columnNameExpression.Name, tables), columnNameExpression.Name),

            LiteralExpression => expression,
            NodeOutputExpression => expression,

            _ => throw new ArgumentOutOfRangeException(nameof(expression))
        };
    }

    private static string ResolveColumn(string column, IReadOnlyDictionary<string, Table> tables)
    {
        (string Alias, Table Table)? selected = null;

        foreach (var (alias, table) in tables)
        {
            if (!table.Columns.Any(x => string.Equals(x.Name, column, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }
            
            if (selected != null)
            {
                throw new Exception($"ambiguous column '{column}'");
            }
                
            selected = (alias, table);
        }
        
        return selected?.Alias ?? throw new Exception($"could not find column '{column}'");
    }
}