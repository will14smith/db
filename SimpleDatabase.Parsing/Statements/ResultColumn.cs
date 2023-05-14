using System;
using SimpleDatabase.Parsing.Expressions;

namespace SimpleDatabase.Parsing.Statements;

public abstract class ResultColumn
{
    public abstract override bool Equals(object obj);
    public abstract override int GetHashCode();

    public class Star : ResultColumn, IEquatable<Star>
    {
        public string? Table { get; }

        public Star(string? table)
        {
            Table = table;
        }
            
        public bool Equals(Star? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Table, other.Table, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is Star other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (Table != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Table) : 0);
        }

        public static bool operator ==(Star? left, Star? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Star? left, Star? right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return "*";
        }
    }

    public class Expression : ResultColumn, IEquatable<Expression>
    {
        public Expressions.Expression Value { get; }
        public string? Alias { get; }

        public Expression(Expressions.Expression value, string? alias)
        {
            Value = value;
            Alias = alias;
        }

        public bool Equals(Expression? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value.Equals(other.Value) && string.Equals(Alias, other.Alias, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is Expression other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Value.GetHashCode() * 397) ^ (Alias != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Alias) : 0);
            }
        }

        public static bool operator ==(Expression? left, Expression? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Expression? left, Expression? right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            if (Alias != null)
            {
                return Alias;
            }

            if (Value is ColumnNameExpression columnName)
            {
                return columnName.Name;
            }
            
            return "<unnamed>";
        }
    }
}