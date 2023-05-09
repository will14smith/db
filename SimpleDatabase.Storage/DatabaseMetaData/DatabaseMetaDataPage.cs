using System;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Storage.DatabaseMetaData;

public class DatabaseMetaDataPage
{
    private readonly Page _page;
    private DatabaseMetaDataPage(Page page)
    {
        _page = page;
    }

    public static DatabaseMetaDataPage Read(Page page)
    {
        if (page.Type != PageType.DatabaseMetaData)
        {
            throw new InvalidOperationException($"Tried to read a {PageType.DatabaseMetaData} page but found a {page.Type} page instead");
        }

        return new DatabaseMetaDataPage(page);
    }

    public static DatabaseMetaDataPage New(Page page)
    {
        return new DatabaseMetaDataPage(page)
        {
            Type = PageType.DatabaseMetaData,
            TableCount = 0,
            NextPageIndex = -1,
        };
    }
    
    public PageId PageId => _page.Id;
    public PageType Type
    {
        get => _page.Type;
        private set => _page.Type = value;
    }
    
    public int TableCount
    {
        get => _page[DatabaseMetaDataPageLayout.TableCountOffset].Read<int>();
        set => _page[DatabaseMetaDataPageLayout.TableCountOffset].Write(value);
    }

    public int NextPageIndex
    {
        get => _page[DatabaseMetaDataPageLayout.NextPageIndexOffset].Read<int>();
        set => _page[DatabaseMetaDataPageLayout.NextPageIndexOffset].Write(value);
    }
    
    public ReadOnlySpan<byte> Data
    {
        get => _page[DatabaseMetaDataPageLayout.DataOffset];
        set => value.CopyTo(_page[DatabaseMetaDataPageLayout.DataOffset]);
    }
}