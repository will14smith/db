namespace SimpleDatabase.Execution.Values
{
    public interface IDeleteTarget
    {
        DeleteResult Delete();
    }

    public abstract class DeleteResult
    {
        public class Success : DeleteResult
        {
            public ICursor NextCursor { get; }

            public Success(ICursor nextCursor)
            {
                NextCursor = nextCursor;
            }
        }
    }
}