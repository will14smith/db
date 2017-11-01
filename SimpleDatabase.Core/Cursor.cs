namespace SimpleDatabase.Core
{
    public class Cursor
    {
        private readonly Table _table;

        public int RowNumber { get; }
        public bool EndOfTable => _table.RowCount == RowNumber;

        public Cursor(Table table, int rowNumber)
        {
            _table = table;
            RowNumber = rowNumber;
        }

        public Cursor Advance()
        {
            return new Cursor(_table, RowNumber + 1);
        }

    }

    public static class TableCursorExtensions
    {
        public static Cursor StartCursor(this Table table)
        {
            return new Cursor(table, 0);
        }
        public static Cursor EndCursor(this Table table)
        {
            return new Cursor(table, table.RowCount);
        }
    }
}
