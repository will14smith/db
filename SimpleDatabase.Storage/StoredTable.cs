using SimpleDatabase.Schemas;

namespace SimpleDatabase.Storage
{
    public class StoredTable
    {
        public Table Table { get; }
        public int RootPageNumber { get; }

        public StoredTable(Table table, int rootPageNumber)
        {
            Table = table;
            RootPageNumber = rootPageNumber;
        }
    }
}
