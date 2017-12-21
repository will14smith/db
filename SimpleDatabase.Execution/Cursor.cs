using SimpleDatabase.Storage.Paging;

namespace SimpleDatabase.Execution
{
    public class Cursor
    {
        public PageId Page { get; }
        public int CellNumber { get; }

        public bool EndOfTable { get; }

        public Cursor(PageId page, int cellNumber, bool endOfTable)
        {
            Page = page;
            CellNumber = cellNumber;

            EndOfTable = endOfTable;
        }
    }
}
