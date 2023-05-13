using System;

namespace SimpleDatabase.Schemas.Types
{
    public abstract class ColumnType : IEquatable<ColumnType>
    {
        public abstract bool Equals(ColumnType? other);
        public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is ColumnType other && Equals(other);

        public abstract override int GetHashCode();

        public static bool operator ==(ColumnType? left, ColumnType? right) => Equals(left, right);
        public static bool operator !=(ColumnType? left, ColumnType? right) => !Equals(left, right);

        /// <summary>
        /// A signed 32-bit integer
        /// </summary>
        public class Integer : ColumnType
        {
            public override bool Equals(ColumnType? other) => other is Integer;
            public override int GetHashCode() => 7;
        }

        /// <summary>
        /// An C string of up to N chars. It is zero terminated
        /// </summary>
        public class String : ColumnType
        {
            public int Length { get; }

            public String(int length)
            {
                Length = length;
            }

            public override bool Equals(ColumnType? other) => other is String otherString && Length == otherString.Length;
            public override int GetHashCode() => 119 ^ Length.GetHashCode();
        }
    }
}