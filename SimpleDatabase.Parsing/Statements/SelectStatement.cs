using System.Collections.Generic;
using SimpleDatabase.Parsing.Expressions;
{
    public class SelectStatement : Statement
    {
        public IReadOnlyList<ResultColumn> Columns { get; }
        public Table Table { get; }
        public Option<Expression> Where { get; }

        public SelectStatement(IReadOnlyList<ResultColumn> columns, Table table, Option<Expression> where)
        {
            Columns = columns;
            Table = table;
            Where = where;
        }
    }
}