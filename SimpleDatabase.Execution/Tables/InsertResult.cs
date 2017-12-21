namespace SimpleDatabase.Execution.Tables
{
    public abstract class InsertResult
    {
        public class Success : InsertResult
        {
            public int Key { get; }

            public Success(int key)
            {
                Key = key;
            }
        }

        public class DuplicateKey : InsertResult
        {
            public int Key { get; }

            public DuplicateKey(int key)
            {
                Key = key;
            }
        }
    }
}
