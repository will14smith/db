using System.Collections.Generic;

namespace SimpleDatabase.Core
{
    public class Table
    {
        public const int PageSize = 4096;
        public const int TableMaxPages = 100;
        public const int RowsPerPage = PageSize / Row.RowSize;
        public const int TableMaxRows = RowsPerPage * TableMaxPages;

        private readonly byte[][] _pages = new byte[TableMaxPages][];
        private int _rowCount;

        public InsertResult Insert(InsertStatement statement)
        {
            if (_rowCount >= TableMaxRows)
            {
                return new InsertResult.TableFull();
            }

            var row = statement.Row;
            var rowNumber = _rowCount++;

            var (page, pageOffset) = GetRowSlot(rowNumber);
            row.Serialize(page, pageOffset);

            return new InsertResult.Success(rowNumber);
        }

        public SelectResult Select(SelectStatement statement)
        {
            var rows = new List<Row>();
            for (var i = 0; i < _rowCount; i++)
            {
                var (page, pageOffset) = GetRowSlot(i);
                var row = Row.Deserialize(page, pageOffset);

                rows.Add(row);
            }

            return new SelectResult.Success(rows);
        }

        private (byte[], int) GetRowSlot(int rowNumber)
        {
            var pageOffset = rowNumber / RowsPerPage;
            var page = _pages[pageOffset] ?? (_pages[pageOffset] = new byte[PageSize]);

            var rowOffset = rowNumber % RowsPerPage;
            var byteOffset = rowOffset * Row.RowSize;

            return (page, byteOffset);
        }
    }

    public abstract class InsertResult
    {
        public class Success : InsertResult
        {
            public int RowNumber { get; }

            public Success(int rowNumber)
            {
                RowNumber = rowNumber;
            }
        }

        public class TableFull : InsertResult
        {
        }
    }

    public abstract class SelectResult
    {
        public class Success : SelectResult
        {
            public IReadOnlyCollection<Row> Rows { get; }

            public Success(IReadOnlyCollection<Row> rows)
            {
                Rows = rows;
            }
        }
    }
}
