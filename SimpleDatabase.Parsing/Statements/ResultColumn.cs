using SimpleDatabase.Utils;namespace SimpleDatabase.Parsing.Statements
{
    public abstract class ResultColumn
    {
        public class Star : ResultColumn
        {
            public Option<string> Table { get; }

            public Star(Option<string> table)
            {
                Table = table;
            }
        }

        public class Expression : ResultColumn
        {
            public Expressions.Expression Value { get; }
            public Option<string> Alias { get; }

            public Expression(Expressions.Expression value, Option<string> alias)
            {
                Value = value;
                Alias = alias;
            }
        }
    }
}