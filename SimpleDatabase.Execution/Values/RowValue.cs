using System.Collections.Generic;

namespace SimpleDatabase.Execution.Values
{
    public class RowValue : Value
    {
        public IReadOnlyCollection<Value> Values { get; }

        public RowValue(IReadOnlyCollection<Value> values)
        {
            Values = values;
        }
    }
}
