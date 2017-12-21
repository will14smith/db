using SimpleDatabase.Schemas;

namespace SimpleDatabase.Execution.Values
{
    public interface IInsertTarget
    {
        InsertTargetResult Insert(Row row);
    }

    public abstract class InsertTargetResult
    {
        public class Success : InsertTargetResult { }
    }
}
