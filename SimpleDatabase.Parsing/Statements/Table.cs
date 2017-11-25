namespace SimpleDatabase.Parsing.Statements
{
    public abstract class Table
    {
        public class TableName : Table
        {
            public string Name { get; }

            public TableName(string name)
            {
                Name = name;
            }
        }
    }
}