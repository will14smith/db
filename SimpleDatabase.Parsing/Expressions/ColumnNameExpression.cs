namespace SimpleDatabase.Parsing.Expressions
{
    public class ColumnNameExpression : Expression
    {
        public string Name { get; }

        public ColumnNameExpression(string name)
        {
            Name = name;
        }
    }
}
