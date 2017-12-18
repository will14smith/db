namespace SimpleDatabase.Storage.Paging
{
    public abstract class PageSource
    {
        public class Heap : PageSource
        {
            public Heap(string tableName)
            {
                TableName = tableName;
            }

            public string TableName { get; }
        }
        public class Index : PageSource
        {
            public Index(string tableName, string indexName)
            {
                TableName = tableName;
                IndexName = indexName;
            }

            public string TableName { get; }
            public string IndexName { get; }
        }
    }
}