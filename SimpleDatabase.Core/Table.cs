using System;
using System.Collections.Generic;
using SimpleDatabase.Core.Paging;

namespace SimpleDatabase.Core
{
    public class Table : IDisposable
    {
        private readonly IPager _pager;
        private int _rowCount;

        public Table(IPager pager)
        {
            _pager = pager;
            _rowCount = pager.RowCount;
        }

        public InsertResult Insert(InsertStatement statement)
        {
            if (_rowCount >= Pager.MaxRows)
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
            var pageOffset = rowNumber / Pager.RowsPerPage;
            var page = _pager.Get(pageOffset);

            var rowOffset = rowNumber % Pager.RowsPerPage;
            var byteOffset = rowOffset * Row.RowSize;

            return (page.Data, byteOffset);
        }

        public void Dispose()
        {
            var fullPageCount = _rowCount / Pager.RowsPerPage;

            for (var i = 0; i < fullPageCount; i++)
            {
                _pager.Flush(i, Pager.PageSize);
            }

            // There may be a partial page to write to the end of the file
            // This should not be needed after we switch to a B-tree
            var additionalRowCount = _rowCount % Pager.RowsPerPage;
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
