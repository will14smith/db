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
    };
    public class SelectStatement : IStatement { }
}
