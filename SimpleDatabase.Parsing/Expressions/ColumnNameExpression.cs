using System;

namespace SimpleDatabase.Parsing.Expressions
{
    public class ColumnNameExpression : Expression, IEquatable<ColumnNameExpression>
    {
        public string Name { get; }

        public ColumnNameExpression(string name)
        {
            Name = name;
        }

        public bool Equals(ColumnNameExpression other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is ColumnNameExpression other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Name) : 0;
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
