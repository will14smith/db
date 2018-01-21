using System;
using System.Collections.Generic;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Serialization;

namespace SimpleDatabase.Storage.Tree
{
    public static class IndexExtensions
    {
        public static (IReadOnlyList<Column> key, IReadOnlyList<Column> data) GetPersistenceColumns(this Index index)
        {
            var key = new List<Column>();
            var data = new List<Column>();

            throw new NotImplementedException();

            return (key, data);
        }

        public static IIndexSerializer CreateSerializer(this Index index)
        {
            var (key, data) = index.GetPersistenceColumns();

            return new IndexSerializer(key, data, new ColumnTypeSerializerFactory());
        }
    }
}