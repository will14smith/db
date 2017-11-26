namespace SimpleDatabase.Parsing.Expressions
{
    public abstract class LiteralExpression : Expression
    {
    }

    public class NumberLiteralExpression : LiteralExpression
    {
        public int Value { get; }

        public NumberLiteralExpression(int value)
        {
            Value = value;
        }
    }

    public class StringLiteralExpression : LiteralExpression
    {
        public string Value { get; }

        public StringLiteralExpression(string value)
        {
            Value = value;
        }
    }
}
