using System;
using SimpleDatabase.Schemas;

namespace SimpleDatabase.Storage.Serialization
{
    public interface IColumnTypeSerializer
    {
        int GetColumnSize();

        ColumnValue ReadColumn(Span<byte> columnStart);
        void WriteColumn(Span<byte> columnStart, ColumnValue value);
    }
}