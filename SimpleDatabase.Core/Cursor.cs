namespace SimpleDatabase.Core
{
    public class Cursor
    {
        private readonly Table _table;

        public int PageNumber { get; }
        public int CellNumber { get; }

        public bool EndOfTable { get; }

        public Cursor(Table table, int pageNumber, int cellNumber, bool endOfTable)
        {
            _table = table;

            PageNumber = pageNumber;
            CellNumber = cellNumber;

            EndOfTable = endOfTable;
        }
    }
}
