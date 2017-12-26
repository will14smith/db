using System;
using System.Collections.Generic;
using SimpleDatabase.Schemas;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Storage.Serialization
{
    public class RowSerializer : IRowSerializer
    {
        private readonly Table _table;
        private readonly ColumnTypeSerializerFactory _columnTypeSerializerFactory;

        private const int MinXidOffset = 0;
        private const int MinXidSize = sizeof(ulong);
        private const int MaxXidOffset = MinXidOffset + MinXidSize;
        private const int MaxXidSize = sizeof(ulong);

        private readonly int _size;
        private readonly IReadOnlyList<int> _offsets;
        private readonly IReadOnlyList<int> _sizes;

        public RowSerializer(Table table, ColumnTypeSerializerFactory columnTypeSerializerFactory)
        {
            _table = table;
            _columnTypeSerializerFactory = columnTypeSerializerFactory;

            _size = MinXidSize + MaxXidSize;

            var offsets = new List<int>();
            var sizes = new List<int>();

            foreach (var column in table.Columns)
            {
                var serializer = _columnTypeSerializerFactory.GetSerializer(column.Type);
                var columnSize = serializer.GetColumnSize();

                offsets.Add(_size);
                sizes.Add(columnSize);

                _size += columnSize;
            }

            _offsets = offsets;
            _sizes = sizes;
        }

        public int GetRowSize()
        {
            return _size;
        }

        public Row ReadRow(Span<byte> rowStart)
        {
            var values = new List<ColumnValue>();

            var minXid = rowStart.Slice(MinXidOffset, MinXidSize).Read<ulong>();
            var maxXid = rowStart.Slice(MaxXidOffset, MaxXidSize).Read<ulong>();

            for (var index = 0; index < _table.Columns.Count; index++)
            {
                values.Add(ReadColumn(rowStart, index));
            }

            return new Row(values, new TransactionId(minXid), maxXid != 0 ? Option.Some(new TransactionId(maxXid)) : Option.None<TransactionId>());
        }
        public void WriteRow(Span<byte> rowStart, Row row)
        {
            // TODO check column types match
            if (row.Values.Count != _table.Columns.Count)
            {
                throw new ArgumentException("Row doesn't have correct number of columns", nameof(row));
            }

            rowStart.Slice(MinXidOffset, MinXidSize).Write(row.MinXid.Id);
            rowStart.Slice(MaxXidOffset, MaxXidSize).Write(row.MaxXid.Select(x => x.Id).OrElse(() => 0ul));

            for (var index = 0; index < _table.Columns.Count; index++)
            {
                WriteColumn(rowStart, index, row.Values[index]);
            }
        }

        public ColumnValue ReadColumn(Span<byte> rowStart, int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= _table.Columns.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(columnIndex));
            }

            var columnStart = rowStart.Slice(_offsets[columnIndex], _sizes[columnIndex]);
            var serializer = _columnTypeSerializerFactory.GetSerializer(_table.Columns[columnIndex].Type);

            return serializer.ReadColumn(columnStart);
        }
        public void WriteColumn(Span<byte> rowStart, int columnIndex, ColumnValue value)
        {
            if (columnIndex < 0 || columnIndex >= _table.Columns.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(columnIndex));
            }

            var columnStart = rowStart.Slice(_offsets[columnIndex], _sizes[columnIndex]);
            var serializer = _columnTypeSerializerFactory.GetSerializer(_table.Columns[columnIndex].Type);

            serializer.WriteColumn(columnStart, value);
        }
    }
}
