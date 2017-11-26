namespace SimpleDatabase.Parsing.Expressions
{
    public class BinaryExpression : Expression
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
    }

    public enum BinaryOperator
    {
        Equal,
        NotEqual
    }
}
