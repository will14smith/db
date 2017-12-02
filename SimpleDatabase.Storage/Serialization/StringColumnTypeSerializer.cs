using System;
using System.Text;
using SimpleDatabase.Schemas;
using SimpleDatabase.Schemas.Types;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Storage.Serialization
{
    public class StringColumnTypeSerializer : IColumnTypeSerializer
    {
        private readonly ColumnType.String _type;

        public StringColumnTypeSerializer(ColumnType.String type)
        {
            _type = type;
        }

        public int GetColumnSize()
        {
            return _type.Length + 1;
        }

        public ColumnValue ReadColumn(Span<byte> columnStart)
        {
            var value = columnStart.Slice(0, _type.Length + 1).ReadString();

            return new ColumnValue(value);
        }

        public void WriteColumn(Span<byte> columnStart, ColumnValue value)
        {
            var str = (string)value.Value;
            var strLength = Encoding.ASCII.GetByteCount(str);
            if (strLength > _type.Length)
            {
                throw new ArgumentOutOfRangeException($"String was longer than max length. {strLength} > {_type.Length}", nameof(value));
            }

            columnStart.WriteString(str);
        }
    }
}