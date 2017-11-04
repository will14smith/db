namespace SimpleDatabase.Core
{
    public class Cursor
    {
        public int PageNumber { get; }
        public int CellNumber { get; }

        public bool EndOfTable { get; }

        public Cursor(int pageNumber, int cellNumber, bool endOfTable)
        {
            PageNumber = pageNumber;
            CellNumber = cellNumber;

            EndOfTable = endOfTable;
        }
    }
}
