using SimpleDatabase.Schemas.Types;

namespace SimpleDatabase.Schemas
{
    public class Column
    {
        public string Name { get; }
        public ColumnType Type { get; }

        public Column(string name, ColumnType type)
        {
            Name = name;
            Type = type;
        }
    }
}