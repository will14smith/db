using System;
using System.Collections.Generic;
using SimpleDatabase.Schemas;

namespace SimpleDatabase.Storage.Serialization
{
    public class IndexSerializer : IIndexSerializer
    {
        private readonly IReadOnlyList<Column> _keyColumns;
        private readonly IReadOnlyList<Column> _dataColumns;

        private readonly ColumnTypeSerializerFactory _columnTypeSerializerFactory;

        private readonly SerializedDataInfo _keyInfo;
        private readonly SerializedDataInfo _dataInfo;

        public IndexSerializer(IReadOnlyList<Column> keyColumns, IReadOnlyList<Column> dataColumns, ColumnTypeSerializerFactory columnTypeSerializerFactory)
        {
            _keyColumns = keyColumns;
            _dataColumns = dataColumns;
            _columnTypeSerializerFactory = columnTypeSerializerFactory;

            _keyInfo = columnTypeSerializerFactory.ComputeSerializedDataInfo(keyColumns);
            _dataInfo = columnTypeSerializerFactory.ComputeSerializedDataInfo(dataColumns);
        }

        public int GetKeySize()
        {
            return _keyInfo.Size;
        }

        public int GetDataSize()
        {
            return _dataInfo.Size;
        }

        public IndexKey ReadKey(Span<byte> data)
        {
            var values = new List<ColumnValue>();

            for (var index = 0; index < _keyColumns.Count; index++)
            {
                var columnData = data.Slice(_keyInfo.Offsets[index], _keyInfo.Sizes[index]);
                var serializer = _columnTypeSerializerFactory.GetSerializer(_keyColumns[index].Type);

                var value = serializer.ReadColumn(columnData);

                values.Add(value);
            }

            return new IndexKey(values);
        }

        public void WriteKey(Span<byte> data, IndexKey row)
        {
            for (var index = 0; index < _keyColumns.Count; index++)
            {
                var columnData = data.Slice(_keyInfo.Offsets[index], _keyInfo.Sizes[index]);
                var serializer = _columnTypeSerializerFactory.GetSerializer(_keyColumns[index].Type);

                serializer.WriteColumn(columnData, row.Values[index]);
            }
        }

        public IndexData ReadData(Span<byte> data)
        {
            var values = new List<ColumnValue>();

            for (var index = 0; index < _dataColumns.Count; index++)
            {
                var columnData = data.Slice(_dataInfo.Offsets[index], _dataInfo.Sizes[index]);
                var serializer = _columnTypeSerializerFactory.GetSerializer(_dataColumns[index].Type);

                var value = serializer.ReadColumn(columnData);

                values.Add(value);
            }

            return new IndexData(values);
        }

        public void WriteData(Span<byte> data, IndexData row)
        {
            for (var index = 0; index < _dataColumns.Count; index++)
            {
                var columnData = data.Slice(_dataInfo.Offsets[index], _dataInfo.Sizes[index]);
                var serializer = _columnTypeSerializerFactory.GetSerializer(_dataColumns[index].Type);

                serializer.WriteColumn(columnData, row.Values[index]);
            }
        }
    }
}