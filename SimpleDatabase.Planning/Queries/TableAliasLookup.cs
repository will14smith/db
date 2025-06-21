using System;
using System.Collections.Generic;
using System.Linq;
using SimpleDatabase.Parsing.Tables;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage;

namespace SimpleDatabase.Planning.Queries;

public class TableAliasLookup
{
    public IReadOnlyDictionary<string, Table> All { get; }
    private TableAliasLookup(IReadOnlyDictionary<string, Table> tables) => All = tables;

    public static TableAliasLookup From(Database database, TableFrom from)
    {
        var tables = new Dictionary<string, Table>
        {
            { from.Table.Alias, database.GetTable(from.Table.Name) }
        };

        foreach (var join in from.Joins)
        {
            tables.Add(join.Table.Alias, database.GetTable(join.Table.Name));
        }
        
        return new TableAliasLookup(tables);
    }

    public (string Alias, Table Table) GetByAlias(string? alias)
    {
        if (alias != null)
        {
            return (alias, All[alias]);
        }

        if (All.Count != 1)
        {
            throw new Exception("ambiguous star in result columns");
        }
        
        var (tableAlias, table) = All.First();
        return (tableAlias, table);
    }
}