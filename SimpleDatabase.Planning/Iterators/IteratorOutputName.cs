using System;
using SimpleDatabase.Parsing.Expressions;

namespace SimpleDatabase.Planning.Iterators
{
    public abstract class IteratorOutputName
    {
        public abstract bool Matches(string name);

        public class TableColumn : IteratorOutputName
        {
            public string Table { get; }
            public string Column { get; }

            public TableColumn(string table, string column)
            {
                Table = table;
                Column = column;
            }

            public override bool Matches(string name)
            {
                // TODO table?
                return string.Equals(Column, name, StringComparison.OrdinalIgnoreCase);
            }
        }

        public class Expression : IteratorOutputName
        {
            public Parsing.Expressions.Expression Expr { get; }

            public Expression(Parsing.Expressions.Expression expr)
            {
                Expr = expr;
            }

            public override bool Matches(string name)
            {
                switch (Expr)
                {
                    case ColumnNameExpression column:
                        return name == column.Name;

                    default: throw new ArgumentOutOfRangeException(nameof(Expr), $"Unhandled type: {Expr.GetType().FullName}");
                }
            }
        }

        public class Constant : IteratorOutputName
        {
            public string Name { get; }

            public Constant(string name)
            {
                Name = name;
            }

            public override bool Matches(string name)
            {
                return string.Equals(Name, name, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}