using System;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Schemas.Serialization
{
    public class IntegerColumnTypeSerializer : IColumnTypeSerializer
    {
        public int GetColumnSize()
        {
            return sizeof(int);
        }

        public ColumnValue ReadColumn(Span<byte> columnStart)
        {
            var value = columnStart.Read<int>();

            return new ColumnValue(value);
        }

        public void WriteColumn(Span<byte> columnStart, ColumnValue value)
        {
            columnStart.Write((int)value.Value);
        }
    }
}