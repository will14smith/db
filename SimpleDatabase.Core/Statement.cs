namespace SimpleDatabase.Core
{
    public interface IStatement
    {
    }

    public class InsertStatement : IStatement
    {
        public Row Row { get; }

        public InsertStatement(Row row)
        {
            Row = row;
        }
    }
    public class DeleteStatement : IStatement
    {
        public int Key { get; }

        public DeleteStatement(int key)
        {
            Key = key;
        }
    }
    public class SelectStatement : IStatement { }
}
