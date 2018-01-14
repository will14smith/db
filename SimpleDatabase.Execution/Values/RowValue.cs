using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleDatabase.Execution.Values
{
    public class RowValue : Value, IEquatable<RowValue>
    {
        public IReadOnlyList<Value> Values { get; }

        public RowValue(IReadOnlyList<Value> values)
        {
            Values = values;
        }

        public bool Equals(RowValue other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return Values.SequenceEqual(other.Values);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;

            return obj is RowValue other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Values.GetHashCode();
        }
    }
}
