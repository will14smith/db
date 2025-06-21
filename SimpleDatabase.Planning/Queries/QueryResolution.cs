using System;
using System.Collections.Generic;
using System.Linq;
using SimpleDatabase.Parsing.Expressions;
using SimpleDatabase.Parsing.Statements;
using SimpleDatabase.Planning.Nodes;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage;

namespace SimpleDatabase.Planning.Queries;

public static class QueryResolution
{
    public static ResolvedQuery Resolve(SelectStatement query, Database database)
    {
        var tables = TableAliasLookup.From(database, query.From);
        var columns = ResolveResultColumns(tables, query.Columns);

        var where = query.Where.Select(x => ResolveExpression(tables, x));
        var ordering = query.Ordering.Select(x => new OrderExpression(ResolveExpression(tables, x.Expression), x.Order)).ToList();
        
        var resolved = new SelectStatement(
            columns,
            query.From,
            where,
            ordering
        );
        
        return new ResolvedQuery(resolved, tables);
    }
    
    public static IReadOnlyList<ResultColumn.Expression> ResolveResultColumns(TableAliasLookup tables, IReadOnlyList<ResultColumn> columns)
    {
        var resolved = new List<ResultColumn.Expression>();

        foreach (var column in columns)
        {
            switch (column)
            {
                case ResultColumn.Star star:
                    if (star.Table == null)
                    {
                        foreach (var (tableAlias, table) in tables.All)
                        {
                            resolved.AddRange(table.Columns.Select(tableColumn => new ResultColumn.Expression(new ColumnNameExpression(tableAlias, tableColumn.Name), tableColumn.Name)));
                        }
                    }
                    else
                    {
                        var (tableAlias, table) = tables.GetByAlias(star.Table);
                        resolved.AddRange(table.Columns.Select(tableColumn => new ResultColumn.Expression(new ColumnNameExpression(tableAlias, tableColumn.Name), tableColumn.Name)));
                    }

                    break;

                case ResultColumn.Expression expression: resolved.Add(new ResultColumn.Expression(ResolveExpression(tables, expression.Value), expression.Alias)); break;
   
                default: throw new ArgumentOutOfRangeException(nameof(column));
            }
        }
        
        return resolved;
    }

    public static Expression ResolveExpression(TableAliasLookup tables, Expression expression)
    {
        return expression switch
        {
            BinaryExpression binaryExpression => new BinaryExpression(
                binaryExpression.Operator,
                ResolveExpression(tables, binaryExpression.Left),
                ResolveExpression(tables, binaryExpression.Right)),
            
            ColumnNameExpression { Table: not null } => expression,
            ColumnNameExpression columnNameExpression => new ColumnNameExpression(ResolveColumn(tables, columnNameExpression.Name), columnNameExpression.Name),

            LiteralExpression => expression,
            NodeOutputExpression => expression,

            _ => throw new ArgumentOutOfRangeException(nameof(expression))
        };
    }

    public static string ResolveColumn(TableAliasLookup tables, string column)
    {
        (string Alias, Table Table)? selected = null;

        foreach (var (alias, table) in tables.All)
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