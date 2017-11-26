using System;

namespace SimpleDatabase.Schemas.Serialization
{
    public interface IRowSerializer
    {
        int GetRowSize();

        Row ReadRow(Span<byte> rowStart);
        void WriteRow(Span<byte> rowStart, Row row);

        ColumnValue ReadColumn(Span<byte> rowStart, int columnIndex);
        void WriteColumn(Span<byte> rowStart, int columnIndex, ColumnValue value);
    }
}