namespace SimpleDatabase.Execution.Values
{
    public interface IDeleteTarget
    {
        DeleteTargetResult Delete();
    }

    public abstract class DeleteTargetResult
    {
        public class Success : DeleteTargetResult
        {
            public ICursor NextCursor { get; }

            public Success(ICursor nextCursor)
            {
                NextCursor = nextCursor;
            }
        }
    }
}