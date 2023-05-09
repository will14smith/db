namespace SimpleDatabase.Storage.Paging
{
    public enum PageType : byte
    {
        Internal = 1,
        Leaf = 2,
        Heap = 3,
        
        DatabaseMetaData = 4,
        TableMetaData = 5,
        Schema = 6,
        SchemaContinuation = 7,
    }
}
