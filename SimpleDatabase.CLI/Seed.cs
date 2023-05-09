using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using SimpleDatabase.Schemas;
using SimpleDatabase.Schemas.Types;
using SimpleDatabase.Storage;
using SimpleDatabase.Storage.Paging;

namespace SimpleDatabase.CLI;

public class Seed
{
    public static void EnsureExists(string folder)
    {
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        using var pager = new Pager(new FolderPageSourceFactory(new FileSystem(), folder));

        using var databaseManager = new DatabaseManager(pager);
        databaseManager.EnsureInitialised();
        
        var table = CreateTable(pager, "table", new[]
        {
            new Column("id", new ColumnType.Integer()),
            new Column("name", new ColumnType.String(31)),
            new Column("email", new ColumnType.String(255)),
        }, new []
        {
            ("pk", new [] { ("id", KeyOrdering.Ascending) }),
            ("k_email", new [] { ("email", KeyOrdering.Ascending) }),
        });
        
        using var tableManager = databaseManager.GetTableManagerFor(table);
        tableManager.EnsureInitialised();
    }
    
    private static Table CreateTable(IPager pager, string name, IReadOnlyList<Column> columns, IEnumerable<(string, (string, KeyOrdering)[])> indexDefs)
    {
        var indices = indexDefs.Select(x => new TableIndex(x.Item1, new KeyStructure(x.Item2.Select(c => (columns.Single(v => v.Name == c.Item1), c.Item2)).ToList(), Array.Empty<Column>()))).ToList();

        return new Table(name, columns, indices);
    }
}