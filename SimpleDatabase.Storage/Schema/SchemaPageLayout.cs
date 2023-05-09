using SimpleDatabase.Storage.Paging;

namespace SimpleDatabase.Storage.MetaData;

public static class SchemaPageLayout
{
    // header
    public const int VersionSize = sizeof(int);
    public const int PreviousSchemaPageIndexSize = sizeof(int);
    public const int SchemaContinuationPageIndexSize = sizeof(int);
    public const int ColumnCountSize = sizeof(byte);
    public const int IndexCountSize = sizeof(byte);

    public const int VersionOffset = PageLayout.PageTypeOffset + PageLayout.PageTypeSize;
    public const int PreviousSchemaPageIndexOffset = VersionOffset + VersionSize;
    public const int SchemaContinuationPageIndexOffset = PreviousSchemaPageIndexOffset + PreviousSchemaPageIndexSize;
    public const int ColumnCountOffset = SchemaContinuationPageIndexOffset + SchemaContinuationPageIndexSize;
    public const int IndexCountOffset = ColumnCountOffset + ColumnCountSize;

    // data
    public const int DataOffset = IndexCountOffset + IndexCountSize;
    public const int DataSize = PageLayout.PageSize - DataOffset;

    // tablename (length u8+data)
    // cols
    //   name (length u8+data)
    //   type (type u8+data)
    // idxs
    //   ncol
    //   ndata
    //   name (length u8+data)
    //   key colindex+order list
    //   data colindex list
}