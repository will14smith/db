using System.Collections.Generic;

namespace SimpleDatabase.Schemas
{
    public class Row
    {
        public IReadOnlyList<ColumnValue> Values { get; }

        public Row(IReadOnlyList<ColumnValue> values)
        {
            Values = values;
        }
    }
}
