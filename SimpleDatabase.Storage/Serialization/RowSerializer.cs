using System;
using System.Collections.Generic;
using SimpleDatabase.Schemas;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Storage.Serialization
{
    public class RowSerializer : IRowSerializer
    {
        private readonly IReadOnlyList<Column> _columns;
        private readonly ColumnTypeSerializerFactory _columnTypeSerializerFactory;

        private const int MinXidOffset = 0;
        private const int MinXidSize = sizeof(ulong);
        private const int MaxXidOffset = MinXidOffset + MinXidSize;
        private const int MaxXidSize = sizeof(ulong);

        private readonly int _size;
        private readonly IReadOnlyList<int> _offsets;
        private readonly IReadOnlyList<int> _sizes;

        public RowSerializer(IReadOnlyList<Column> columns, ColumnTypeSerializerFactory columnTypeSerializerFactory)
        {
            _columns = columns;
            _columnTypeSerializerFactory = columnTypeSerializerFactory;

            _size = MinXidSize + MaxXidSize;

            var offsets = new List<int>();
            var sizes = new List<int>();

            foreach (var column in _columns)
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

            var (minXid, maxXid) = ReadXid(rowStart);

            for (var index = 0; index < _columns.Count; index++)
            {
                values.Add(ReadColumn(rowStart, index));
            }

            return new Row(values, minXid, maxXid);
        }
        public void WriteRow(Span<byte> rowStart, Row row)
        {
            // TODO check column types match
            if (row.Values.Count != _columns.Count)
            {
                throw new ArgumentException("Row doesn't have correct number of columns", nameof(row));
            }

            rowStart.Slice(MinXidOffset, MinXidSize).Write(row.MinXid.Id);
            rowStart.Slice(MaxXidOffset, MaxXidSize).Write(row.MaxXid.Select(x => x.Id).OrElse(() => 0ul));

            for (var index = 0; index < _columns.Count; index++)
            {
                WriteColumn(rowStart, index, row.Values[index]);
            }
        }

        (TransactionId min, Option<TransactionId> max) IRowSerializer.ReadXid(Span<byte> rowStart)
        {
            return ReadXid(rowStart);
        }
        public static (TransactionId min, Option<TransactionId> max) ReadXid(Span<byte> rowStart)
        {
            var min = rowStart.Slice(MinXidOffset, MinXidSize).Read<ulong>();
            var max = rowStart.Slice(MaxXidOffset, MaxXidSize).Read<ulong>();

            return (new TransactionId(min), max != 0 ? TransactionId.Some(max) : TransactionId.None());
        }

        public ColumnValue ReadColumn(Span<byte> rowStart, int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= _columns.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(columnIndex));
            }

            var columnStart = rowStart.Slice(_offsets[columnIndex], _sizes[columnIndex]);
            var serializer = _columnTypeSerializerFactory.GetSerializer(_columns[columnIndex].Type);

            return serializer.ReadColumn(columnStart);
        }
        public void WriteColumn(Span<byte> rowStart, int columnIndex, ColumnValue value)
        {
            if (columnIndex < 0 || columnIndex >= _columns.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(columnIndex));
            }

            var columnStart = rowStart.Slice(_offsets[columnIndex], _sizes[columnIndex]);
            var serializer = _columnTypeSerializerFactory.GetSerializer(_columns[columnIndex].Type);

            serializer.WriteColumn(columnStart, value);
        }
    }
}
