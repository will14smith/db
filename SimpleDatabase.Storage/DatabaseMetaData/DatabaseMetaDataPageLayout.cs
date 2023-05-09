using SimpleDatabase.Storage.Paging;

namespace SimpleDatabase.Storage.DatabaseMetaData;

public static class DatabaseMetaDataPageLayout
{
    // header
    public const int TableCountSize = sizeof(uint);
    public const int NextPageIndexSize = sizeof(int);
    
    public const int TableCountOffset = PageLayout.PageTypeOffset + PageLayout.PageTypeSize;
    public const int NextPageIndexOffset = TableCountOffset + TableCountSize;

    // data
    public const int DataOffset = NextPageIndexOffset + NextPageIndexSize;
    public const int DataSize = PageLayout.PageSize - DataOffset;

    public static int TableEntrySize(int tableNameLength) => sizeof(int) + 1 + tableNameLength;

    // data format:
    // table list
    //   name (length u8+data)
    //   current schema page index
}