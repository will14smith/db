using System;

namespace SimpleDatabase.Schemas
{
    public class ColumnValue : IEquatable<ColumnValue>
    {
        public object Value { get; }

        public ColumnValue(object value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value?.ToString();
        }

        public bool Equals(ColumnValue other)
        {
            if (other is null) return false;
            return ReferenceEquals(this, other) || Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is ColumnValue value && Equals(value);
        }

        public override int GetHashCode()
        {
            return Value != null ? Value.GetHashCode() : 0;
        }

        public static bool operator ==(ColumnValue left, ColumnValue right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ColumnValue left, ColumnValue right)
        {
            return !Equals(left, right);
        }
    }
}