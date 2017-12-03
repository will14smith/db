using SimpleDatabase.Parsing.Expressions;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Parsing.Statements
{
    public class DeleteStatement : Statement
    {
        public string Table { get; }
        public Option<Expression> Where { get; }

        public DeleteStatement(string table, Option<Expression> where)
        {
            Table = table;
            Where = where;
        }
    }
}
