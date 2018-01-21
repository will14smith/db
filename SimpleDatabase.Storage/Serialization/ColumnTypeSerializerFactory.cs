using System;
using System.Collections.Generic;
using SimpleDatabase.Schemas;
using SimpleDatabase.Schemas.Types;

namespace SimpleDatabase.Storage.Serialization
{
    public class ColumnTypeSerializerFactory
    {
        public IColumnTypeSerializer GetSerializer(ColumnType type)
        {
            switch (type)
            {
                case ColumnType.Integer _:
                    return new IntegerColumnTypeSerializer();
                case ColumnType.String str:
                    return new StringColumnTypeSerializer(str);
                default:
                    throw new NotImplementedException(type.GetType().FullName);
            }
        }

        public SerializedDataInfo ComputeSerializedDataInfo(IReadOnlyList<Column> columns, int initialOffset = 0)
        {
            var size = initialOffset;

            var offsets = new List<int>();
            var sizes = new List<int>();

            foreach (var column in columns)
            {
                var serializer = GetSerializer(column.Type);
                var columnSize = serializer.GetColumnSize();

                offsets.Add(size);
                sizes.Add(columnSize);

                size += columnSize;
            }

            return new SerializedDataInfo(size, offsets, sizes);
        }
    }

    public class SerializedDataInfo
    {
        public SerializedDataInfo(int size, IReadOnlyList<int> offsets, IReadOnlyList<int> sizes)
        {
            Size = size;
            Offsets = offsets;
            Sizes = sizes;
        }

        public int Size { get; }

        public IReadOnlyList<int> Offsets { get; }
        public IReadOnlyList<int> Sizes { get; }
    }
}
