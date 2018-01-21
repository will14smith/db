namespace SimpleDatabase.Execution.Tables
{
    public abstract class InsertResult
    {
        public class Success : InsertResult { }

        public class DuplicateKey : InsertResult { }
    }
}
