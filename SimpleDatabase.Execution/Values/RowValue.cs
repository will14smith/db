using System.Collections.Generic;

namespace SimpleDatabase.Execution.Values
{
    public class RowValue : Value
    {
        public IReadOnlyList<Value> Values { get; }

        public RowValue(IReadOnlyList<Value> values)
        {
            Values = values;
        }
    }
}
