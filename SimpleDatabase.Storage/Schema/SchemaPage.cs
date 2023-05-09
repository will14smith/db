using System;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Storage.MetaData;

public class SchemaPage
{
    private readonly Page _page;
    private SchemaPage(Page page)
    {
        _page = page;
    }

    public static SchemaPage Read(Page page)
    {
        if (page.Type != PageType.Schema)
        {
            throw new InvalidOperationException($"Tried to read a {PageType.Schema} page but found a {page.Type} page instead");
        }

        return new SchemaPage(page);
    }
    
    public static SchemaPage New(Page page)
    {
        return new SchemaPage(page)
        {
            Type = PageType.Schema,
            Version = -1,
            PreviousSchemaPageIndex = -1,
            SchemaContinuationPageIndex = -1,
            ColumnCount = 0,
            IndexCount = 0,
        };
    }
    
    public PageId PageId => _page.Id;
    public PageType Type
    {
        get => _page.Type;
        private set => _page.Type = value;
    }
    
    public int Version
    {
        get => _page[SchemaPageLayout.VersionOffset].Read<int>();
        set => _page[SchemaPageLayout.VersionOffset].Write(value);
    }
    public int PreviousSchemaPageIndex
    {
        get => _page[SchemaPageLayout.PreviousSchemaPageIndexOffset].Read<int>();
        set => _page[SchemaPageLayout.PreviousSchemaPageIndexOffset].Write(value);
    }
    public int SchemaContinuationPageIndex
    {
        get => _page[SchemaPageLayout.SchemaContinuationPageIndexOffset].Read<int>();
        set => _page[SchemaPageLayout.SchemaContinuationPageIndexOffset].Write(value);
    }
    public byte ColumnCount
    {
        get => _page[SchemaPageLayout.ColumnCountOffset].Read<byte>();
        set => _page[SchemaPageLayout.ColumnCountOffset].Write(value);
    }
    public byte IndexCount
    {
        get => _page[SchemaPageLayout.IndexCountOffset].Read<byte>();
        set => _page[SchemaPageLayout.IndexCountOffset].Write(value);
    }

    public ReadOnlySpan<byte> Data
    {
        get => _page[SchemaPageLayout.DataOffset];
        set => value.CopyTo(_page[SchemaPageLayout.DataOffset]);
    }
}