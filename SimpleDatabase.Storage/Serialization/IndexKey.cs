using System.Collections.Generic;
using SimpleDatabase.Schemas;

namespace SimpleDatabase.Storage.Serialization
{
    public class IndexKey
    {
        public IReadOnlyList<ColumnValue> Values { get; }

        public IndexKey(IReadOnlyList<ColumnValue> values)
        {
            Values = values;
        }
    }
}