using SimpleDatabase.Storage.Paging;

namespace SimpleDatabase.Storage.MetaData;

public static class SchemaContinuationPageLayout
{
    // header
    public const int SchemaPageIndexSize = sizeof(uint);
    public const int SchemaContinuationPageIndexSize = sizeof(int);

    
    public const int SchemaPageIndexOffset = PageLayout.PageTypeOffset + PageLayout.PageTypeSize;
    public const int SchemaContinuationPageIndexOffset = SchemaPageIndexOffset + SchemaPageIndexSize;

    // data
    public const int DataOffset = SchemaContinuationPageIndexOffset + SchemaContinuationPageIndexSize;
    public const int DataSize = PageLayout.PageSize - DataOffset;

    // same structure as SchemaPageLayout
}