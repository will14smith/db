using System;

namespace SimpleDatabase.Parsing.Expressions
{
    public class ColumnNameExpression : Expression, IEquatable<ColumnNameExpression>
    {
        public string? Table { get; }
        public string Name { get; }

        public ColumnNameExpression(string name) : this(null, name) { }
        public ColumnNameExpression(string? table, string name)
        {
            Table = table;
            Name = name;
        }

        public override string ToString()
        {
            if (Table != null) return $"{Table}.{Name}";
            return Name;
        }

        public bool Equals(ColumnNameExpression other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Table, other.Table, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is ColumnNameExpression other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Table, Name);
        }

        public static bool operator ==(ColumnNameExpression left, ColumnNameExpression right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ColumnNameExpression left, ColumnNameExpression right)
        {
            return !Equals(left, right);
        }
    }
}
