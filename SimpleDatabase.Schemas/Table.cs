using System.Collections.Generic;

namespace SimpleDatabase.Schemas
{
    public class Table
    {
        public string Name { get; }
        public IReadOnlyList<Column> Columns { get; }

        public Table(string name, IReadOnlyList<Column> columns)
        {
            Name = name;
            Columns = columns;
        }
    }
}
