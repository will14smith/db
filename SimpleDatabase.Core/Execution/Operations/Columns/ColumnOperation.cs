namespace SimpleDatabase.Core.Execution.Operations
{
    /// <summary>
    /// ..., T : ReadOnlyCursor -> ..., value
    /// 
    /// Reads the nth column of the row pointed at by the cursor and pushes it onto the stack
    /// </summary>
    public class ColumnOperation : Operation
    {
        public int ColumnIndex { get; }

        public ColumnOperation(int columnIndex)
        {
            ColumnIndex = columnIndex;
        }
    }
}
