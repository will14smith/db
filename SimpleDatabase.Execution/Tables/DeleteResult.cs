namespace SimpleDatabase.Execution.Tables
{
    public abstract class DeleteResult
    {
        public class Success : DeleteResult
        {
            public Success()
            {
            }
        }

        public class KeyNotFound : DeleteResult { }
    }
}
