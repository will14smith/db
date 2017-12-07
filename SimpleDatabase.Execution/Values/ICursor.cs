using SimpleDatabase.Schemas;

namespace SimpleDatabase.Execution.Values
{
    public interface ICursor
    {
        bool EndOfTable { get; }

        ICursor First();
        ICursor Next();

        int Key();
        ColumnValue Column(int index);
    }
}