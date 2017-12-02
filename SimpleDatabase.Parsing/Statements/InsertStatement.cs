using System;
using System.Collections.Generic;
using System.Text;
using SimpleDatabase.Parsing.Expressions;

namespace SimpleDatabase.Parsing.Statements
{
    public class InsertStatement : Statement
    {
        public string Table { get; }
        public IReadOnlyList<string> Columns { get; }
        public IReadOnlyList<IReadOnlyList<Expression>> Values { get; }

        public InsertStatement(string table, IReadOnlyList<string> columns, IReadOnlyList<IReadOnlyList<Expression>> values)
        {
            Table = table;
            Columns = columns;
            Values = values;
        }
    }
}
