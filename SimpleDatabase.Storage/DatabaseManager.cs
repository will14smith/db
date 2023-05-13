using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.DatabaseMetaData;
using SimpleDatabase.Storage.MetaData;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Storage;

public class DatabaseManager : IDisposable
{
    private readonly IPager _pager;
    
    private readonly DatabaseMetaDataReader _databaseMetaDataReader;
    private readonly DatabaseMetaDataWriter _databaseMetaDataWriter;

    private readonly SchemaReader _schemaReader;
    private readonly SchemaWriter _schemaWriter;

    public DatabaseManager(IPager pager)
    {
        _pager = pager;

        var databasePager = new SourcePager(pager, PageSource.Database.Instance);
        
        _databaseMetaDataReader = new DatabaseMetaDataReader(databasePager);
        _databaseMetaDataWriter = new DatabaseMetaDataWriter(databasePager);

        _schemaReader = new SchemaReader(databasePager);
        _schemaWriter = new SchemaWriter(databasePager);
    }

    public void EnsureInitialised() => _databaseMetaDataWriter.EnsureInitialised();

    public IReadOnlyCollection<Table> GetAllTables() =>
        _databaseMetaDataReader.ReadTables()
            .Select(metaDataTable => metaDataTable.CurrentSchemaPageId)
            .Select(schemaPageId => _schemaReader.Read(schemaPageId))
            .ToList();

    public Option<Table> TryGetTableSchema(string tableName)
    {
        var metaDataTable = _databaseMetaDataReader.ReadTables()
            .FirstOrDefault(metaDataTable => metaDataTable.Name == tableName);
        if (metaDataTable == null)
        {
            return Option.None();
        }

        var table = _schemaReader.Read(metaDataTable.CurrentSchemaPageId);
        return Option.Some(table);
    }
    
    public void SetTableSchema(Table table)
    {
        var schemaPage = _schemaWriter.Write(table);
        
        _databaseMetaDataWriter.SetTableSchema(table.Name, schemaPage.PageId);
    }

    public TableManager GetTableManagerFor(string tableName)
    {
        var table = GetAllTables().First(x => x.Name == tableName);

        return GetTableManagerFor(table);
    }
    public TableManager GetTableManagerFor(Table table)
    {
        var tablePager = new SourcePager(_pager, new PageSource.Table(table.Name));
        return new TableManager(this, tablePager, table);
    }

    public void Dispose() { }
}