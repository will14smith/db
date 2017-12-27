using System;
using SimpleDatabase.Schemas;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Storage.Serialization
{
    public interface IRowSerializer
    {
        int GetRowSize();

        Row ReadRow(Span<byte> rowStart);
        void WriteRow(Span<byte> rowStart, Row row);

        (TransactionId min, Option<TransactionId> max) ReadXid(Span<byte> rowStart);

        ColumnValue ReadColumn(Span<byte> rowStart, int columnIndex);
        void WriteColumn(Span<byte> rowStart, int columnIndex, ColumnValue value);
    }
}