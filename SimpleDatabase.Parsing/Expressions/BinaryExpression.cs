using System;

namespace SimpleDatabase.Parsing.Expressions
{
    public class BinaryExpression : Expression, IEquatable<BinaryExpression>
    {
        public BinaryOperator Operator { get; }

        public Expression Left { get; }
        public Expression Right { get; }

        public BinaryExpression(BinaryOperator op, Expression left, Expression right)
        {
            Operator = op;
            Left = left;
            Right = right;
        }

        public bool Equals(BinaryExpression other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Operator == other.Operator && Left.Equals(other.Left) && Right.Equals(other.Right);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is BinaryExpression other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) Operator;
                hashCode = (hashCode * 397) ^ Left.GetHashCode();
                hashCode = (hashCode * 397) ^ Right.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(BinaryExpression left, BinaryExpression right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BinaryExpression left, BinaryExpression right)
        {
            return !Equals(left, right);
        }
    }

    public enum BinaryOperator
    {
        Equal,
        NotEqual
    }
}
