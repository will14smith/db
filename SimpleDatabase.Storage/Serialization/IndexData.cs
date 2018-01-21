using System.Collections.Generic;
using SimpleDatabase.Schemas;

namespace SimpleDatabase.Storage.Serialization
{
    public class IndexData
    {
        public IReadOnlyList<ColumnValue> Values { get; }

        public IndexData(IReadOnlyList<ColumnValue> values)
        {
            Values = values;
        }
    }
}