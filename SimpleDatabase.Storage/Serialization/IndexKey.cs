using System;
using System.Collections.Generic;
using System.Linq;
using SimpleDatabase.Schemas;

namespace SimpleDatabase.Storage.Serialization
{
    public class IndexKey : IEquatable<IndexKey>, IComparable<IndexKey>
    {
        public IReadOnlyList<ColumnValue> Values { get; }

        public IndexKey(IReadOnlyList<ColumnValue> values)
        {
            Values = values;
        }

        public bool Equals(IndexKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return Values.SequenceEqual(other.Values);
        }

        public int CompareTo(IndexKey other)
        {
            if (Values.Count != other.Values.Count)
            {
                throw new InvalidOperationException();
            }

            for (var index = 0; index < Values.Count; index++)
            {
                var value1 = Values[index];
                var value2 = other.Values[index];

                // TODO better comparer?
                var comparison = Comparer<object>.Default.Compare(value1.Value, value2.Value);
                if (comparison != 0)
                {
                    return comparison;
                }
            }

            return 0;
        }

        #region boilerplate
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IndexKey) obj);
        }

        public override int GetHashCode()
        {
            return (Values != null ? Values.GetHashCode() : 0);
        }

        public static bool operator ==(IndexKey left, IndexKey right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(IndexKey left, IndexKey right)
        {
            return !Equals(left, right);
        }

        public static bool operator <(IndexKey left, IndexKey right)
        {
            return Comparer<IndexKey>.Default.Compare(left, right) < 0;
        }

        public static bool operator >(IndexKey left, IndexKey right)
        {
            return Comparer<IndexKey>.Default.Compare(left, right) > 0;
        }

        public static bool operator <=(IndexKey left, IndexKey right)
        {
            return Comparer<IndexKey>.Default.Compare(left, right) <= 0;
        }

        public static bool operator >=(IndexKey left, IndexKey right)
        {
            return Comparer<IndexKey>.Default.Compare(left, right) >= 0;
        }
        #endregion
    }
}