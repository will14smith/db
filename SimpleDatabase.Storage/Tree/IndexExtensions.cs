using System;
using System.Collections.Generic;
using SimpleDatabase.Schemas;

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
    }
}