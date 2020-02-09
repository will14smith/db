using System;

namespace SimpleDatabase.Parsing.Statements
{
    public abstract class Table
    {
        public abstract override bool Equals(object obj);
        public abstract override int GetHashCode();

        public class TableName : Table, IEquatable<TableName>
        {
            public string Name { get; }

            public TableName(string name)
            {
                Name = name;
            }

            public bool Equals(TableName? other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);
            }

            public override bool Equals(object? obj)
            {
                return ReferenceEquals(this, obj) || obj is TableName other && Equals(other);
            }

            public override int GetHashCode()
            {
                return StringComparer.OrdinalIgnoreCase.GetHashCode(Name);
            }

            public static bool operator ==(TableName? left, TableName? right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(TableName? left, TableName? right)
            {
                return !Equals(left, right);
            }
        }
    }
}