namespace SimpleDatabase.Execution.Trees
{
    public abstract class TreeDeleteResult
    {
        public class Success : TreeDeleteResult
        {
            public int Key { get; }

            public Success(int key)
            {
                Key = key;
            }
        }

        public class KeyNotFound : TreeDeleteResult
        {
            public int Key { get; }

            public KeyNotFound(int key)
            {
                Key = key;
            }
        }
    }
}
