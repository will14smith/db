namespace SimpleDatabase.Core
{
    public interface IStatement
    {
    }

    public class InsertStatement : IStatement { }
    public class SelectStatement : IStatement { }
}
