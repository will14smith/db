using System;
using System.Collections.Generic;
using SimpleDatabase.Core.Paging;

namespace SimpleDatabase.Core
{
    public class Table : IDisposable
    {
        private readonly IPager _pager;

        public int RowCount { get; private set; }

        public Table(IPager pager)
        {
            _pager = pager;
            RowCount = pager.RowCount;
        }

        public InsertResult Insert(InsertStatement statement)
        {
            if (RowCount >= Pager.MaxRows)
            {
                return new InsertResult.TableFull();
            }

            var row = statement.Row;
            var cursor = this.EndCursor();

            var (page, pageOffset) = GetRowSlot(cursor);
            row.Serialize(page, pageOffset);
            RowCount++;

            return new InsertResult.Success(cursor.RowNumber);
        }

        public SelectResult Select(SelectStatement statement)
        {
            var cursor = this.StartCursor();

            var rows = new List<Row>();
            while (!cursor.EndOfTable)
            {
                var (page, pageOffset) = GetRowSlot(cursor);
                var row = Row.Deserialize(page, pageOffset);
                cursor = cursor.Advance();

                rows.Add(row);
            }

            return new SelectResult.Success(rows);
        }

        private (byte[], int) GetRowSlot(Cursor cursor)
        {
            var pageOffset = cursor.RowNumber / Pager.RowsPerPage;
            var page = _pager.Get(pageOffset);

            var rowOffset = cursor.RowNumber % Pager.RowsPerPage;
            var byteOffset = rowOffset * Row.RowSize;

            return (page.Data, byteOffset);
        }

        public void Dispose()
        {
            var fullPageCount = RowCount / Pager.RowsPerPage;

            for (var i = 0; i < fullPageCount; i++)
            {
                _pager.Flush(i, Pager.PageSize);
            }

            // There may be a partial page to write to the end of the file
            // This should not be needed after we switch to a B-tree
            var additionalRowCount = RowCount % Pager.RowsPerPage;
            if (additionalRowCount > 0)
            {
                var pageIndex = fullPageCount;
                _pager.Flush(pageIndex, additionalRowCount * Row.RowSize);
            }
            
            _pager?.Dispose();
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
