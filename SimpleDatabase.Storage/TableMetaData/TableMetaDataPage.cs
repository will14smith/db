using System;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Storage.TableMetaData;

public class TableMetaDataPage
{
    private readonly Page _page;
    private TableMetaDataPage(Page page)
    {
        _page = page;
    }

    public static TableMetaDataPage Read(Page page)
    {
        if (page.Type != PageType.TableMetaData)
        {
            throw new InvalidOperationException($"Tried to read a {PageType.TableMetaData} page but found a {page.Type} page instead");
        }

        return new TableMetaDataPage(page);
    }

    public static TableMetaDataPage New(Page page)
    {
        return new TableMetaDataPage(page)
        {
            Type = PageType.TableMetaData,
            RootHeapPageIndex = -1,
        };
    }
    
    public PageId PageId => _page.Id;
    public PageType Type
    {
        get => _page.Type;
        private set => _page.Type = value;
    }
    
    public int RootHeapPageIndex
    {
        get => _page[TableMetaDataPageLayout.RootHeapPageIndexOffset].Read<int>();
        set => _page[TableMetaDataPageLayout.RootHeapPageIndexOffset].Write(value);
    }

    public int this[int index]
    {
        get => _page[TableMetaDataPageLayout.IndexRootPageIndexOffset + index * TableMetaDataPageLayout.IndexRootPageIndexSize].Read<int>();
        set => _page[TableMetaDataPageLayout.IndexRootPageIndexOffset + index * TableMetaDataPageLayout.IndexRootPageIndexSize].Write(value);
    }
}