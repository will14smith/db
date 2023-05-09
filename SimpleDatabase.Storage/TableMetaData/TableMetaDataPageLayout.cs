using SimpleDatabase.Storage.Paging;

namespace SimpleDatabase.Storage.TableMetaData;

public static class TableMetaDataPageLayout
{
    public const int RootHeapPageIndexSize = sizeof(int);
    public const int IndexRootPageIndexSize = sizeof(int);

    public const int RootHeapPageIndexOffset = PageLayout.PageTypeOffset + PageLayout.PageTypeSize;
    public const int IndexRootPageIndexOffset = RootHeapPageIndexOffset + RootHeapPageIndexSize;
}