using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Paging;

namespace SimpleDatabase.Storage
{
    public class StoredTable
    {
        public Table Table { get; }
        public PageId RootPageId { get; }

        public StoredTable(Table table, int rootPageIndex)
        {
            Table = table;
            RootPageId = new PageId(PageStorageType.Tree, rootPageIndex);
        }
        public StoredTable(Table table, PageId rootPageId)
        {
            Table = table;
            RootPageId = rootPageId;
        }
    }
}
