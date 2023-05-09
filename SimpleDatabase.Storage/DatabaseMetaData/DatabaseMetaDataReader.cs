using System;
using System.Collections.Generic;
using SimpleDatabase.Storage.MetaData;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Storage.DatabaseMetaData;

public class DatabaseMetaDataReader
{
    private readonly ISourcePager _pager;
    
    public DatabaseMetaDataReader(ISourcePager pager)
    {
        _pager = pager;
    }

    public IReadOnlyCollection<DatabaseMetaDataTable> ReadTables()
    {
        var tables = new List<DatabaseMetaDataTable>();
        
        var page = DatabaseMetaDataPage.Read(_pager.Get(0));
        while (true)
        {
            tables.AddRange(ReadTables(page));

            if (page.NextPageIndex < 0)
            {
                break;
            }
            
            page = DatabaseMetaDataPage.Read(_pager.Get(page.NextPageIndex));
        }

        return tables;
    }
    
    private static IEnumerable<DatabaseMetaDataTable> ReadTables(DatabaseMetaDataPage page)
    {
        var tables = new List<DatabaseMetaDataTable>();

        var data = page.Data;
        
        for (var i = 0; i < page.TableCount; i++)
        {
            var name = data.ReadU8LengthString();
            data = data[(name.Length + 1)..];
            var currentSchemaPageIndex = new PageId(PageSource.Database.Instance, data.Read<int>());
            data = data[sizeof(int)..];
            
            tables.Add(new DatabaseMetaDataTable(name, currentSchemaPageIndex));
        }
        
        return tables;
    }
}

public class DatabaseMetaDataTable
{
    public DatabaseMetaDataTable(string name, PageId currentSchemaPageId)
    {
        Name = name;
        CurrentSchemaPageId = currentSchemaPageId;
    }

    public string Name { get; }
    public PageId CurrentSchemaPageId { get; }
}