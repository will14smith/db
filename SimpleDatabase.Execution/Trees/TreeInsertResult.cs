namespace SimpleDatabase.Execution.Trees
{
    public abstract class TreeInsertResult
    {
        public class Success : TreeInsertResult
        {
            public int Key { get; }

            public Success(int key)
            {
                Key = key;
            }
        }

        public class DuplicateKey : TreeInsertResult
        {
            public int Key { get; }

            public DuplicateKey(int key)
            {
                Key = key;
            }
        }
    }
}
