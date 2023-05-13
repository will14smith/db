using System;

namespace SimpleDatabase.Parsing.Expressions
{
    public abstract class LiteralExpression : Expression
    {
    }

    public class NumberLiteralExpression : LiteralExpression, IEquatable<NumberLiteralExpression>
    {
        public int Value { get; }

        public NumberLiteralExpression(int value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public bool Equals(NumberLiteralExpression other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is NumberLiteralExpression other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value;
        }

        public static bool operator ==(NumberLiteralExpression left, NumberLiteralExpression right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(NumberLiteralExpression left, NumberLiteralExpression right)
        {
            return !Equals(left, right);
        }
    }

    public class StringLiteralExpression : LiteralExpression, IEquatable<StringLiteralExpression>
    {
        public string Value { get; }

        public StringLiteralExpression(string value)
        {
            Value = value;
        }
        
        public override string ToString()
        {
            return $"'{Value}'";
        }

        public bool Equals(StringLiteralExpression other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is StringLiteralExpression other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (Value != null ? Value.GetHashCode() : 0);
        }

        public static bool operator ==(StringLiteralExpression left, StringLiteralExpression right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(StringLiteralExpression left, StringLiteralExpression right)
        {
            return !Equals(left, right);
        }
    }
}
