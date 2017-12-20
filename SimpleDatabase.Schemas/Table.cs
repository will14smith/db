using System.Collections.Generic;

namespace SimpleDatabase.Schemas
{
    public class Table
    {
        public string Name { get; }
        public IReadOnlyList<Column> Columns { get; }
        public IReadOnlyList<Index> Indices { get; }

        public Table(string name, IReadOnlyList<Column> columns, IReadOnlyList<Index> indices)
        {
            Name = name;
            Columns = columns;
            Indices = indices;
        }
    }
}
