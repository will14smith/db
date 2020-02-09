using System.Collections.Generic;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Schemas
{
    public class Table
    {
        public string Name { get; }
        public IReadOnlyList<Column> Columns { get; }
        public IReadOnlyList<TableIndex> Indices { get; }

        public Table(string name, IReadOnlyList<Column> columns, IReadOnlyList<TableIndex> indices)
        {
            Name = name;
            Columns = columns;
            Indices = indices;
        }
    }

    public static class TableExtensions
    {
        public static int? IndexOf(this Table table, Column column)
        {
            for (var index = 0; index < table.Columns.Count; index++)
            {
                var tableColumn = table.Columns[index];
                if (tableColumn.Name == column.Name)
                {
                    return index;
                }
            }

            return null;
        }
    }
}
