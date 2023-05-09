using System;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Storage.MetaData;

public class SchemaContinuationPage
{
    private readonly Page _page;
    private SchemaContinuationPage(Page page)
    {
        _page = page;
    }

    public static SchemaContinuationPage Read(Page page)
    {
        if (page.Type != PageType.SchemaContinuation)
        {
            throw new InvalidOperationException($"Tried to read a {PageType.SchemaContinuation} page but found a {page.Type} page instead");
        }

        return new SchemaContinuationPage(page);
    }

    public static SchemaContinuationPage New(Page page)
    {
        return new SchemaContinuationPage(page)
        {
            Type = PageType.SchemaContinuation,
            SchemaPageIndex = -1,
            SchemaContinuationPageIndex = -1,
        };
    }
    
    public PageId PageId => _page.Id;
    public PageType Type
    {
        get => _page.Type;
        private set => _page.Type = value;
    }

    public int SchemaPageIndex
    {
        get => _page[SchemaContinuationPageLayout.SchemaPageIndexOffset].Read<int>();
        set => _page[SchemaContinuationPageLayout.SchemaPageIndexOffset].Write(value);
    }
    public int SchemaContinuationPageIndex
    {
        get => _page[SchemaContinuationPageLayout.SchemaContinuationPageIndexOffset].Read<int>();
        set => _page[SchemaContinuationPageLayout.SchemaContinuationPageIndexOffset].Write(value);
    }

    public Span<byte> Data
    {
        get => _page[SchemaContinuationPageLayout.DataOffset];
        set => value.CopyTo(_page[SchemaContinuationPageLayout.DataOffset]);
    }
}