namespace SimpleDatabase.Execution
{
    public abstract class DeleteResult
    {
        public class Success : DeleteResult
        {
            public int Key { get; }

            public Success(int key)
            {
                Key = key;
            }
        }

        public class KeyNotFound : DeleteResult
        {
            public int Key { get; }

            public KeyNotFound(int key)
            {
                Key = key;
            }
        }
    }
}
