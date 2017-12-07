using SimpleDatabase.Schemas;

namespace SimpleDatabase.Execution.Values
{
    public interface IInsertTarget
    {
        InsertResult Insert(Row row);
    }

    public abstract class InsertResult
    {
        public class Success : InsertResult { }
    }
}
