using System;
using System.Collections.Generic;
using System.Linq;
using SimpleDatabase.Schemas;
using SimpleDatabase.Schemas.Types;
using SimpleDatabase.Storage.Serialization;

namespace SimpleDatabase.Storage.Tree
{
    public static class IndexExtensions
    {
        public static (IReadOnlyList<Column> key, IReadOnlyList<Column> data) GetPersistenceColumns(this TableIndex index)
        {
            var key = index.Structure.Keys.Select(col => col.Item1).ToList();

            var data = new List<Column>
            {
                new Column("__heapkey", new ColumnType.Integer())
            };
            data.AddRange(index.Structure.Data);

            return (key, data);
        }

        public static IIndexSerializer CreateSerializer(this TableIndex index)
        {
            var (key, data) = index.GetPersistenceColumns();

            return new IndexSerializer(key, data, new ColumnTypeSerializerFactory());
        }
    }
}