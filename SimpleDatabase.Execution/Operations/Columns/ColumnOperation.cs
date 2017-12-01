namespace SimpleDatabase.Execution.Operations.Columns
{
    /// <summary>
    /// ..., T : ReadOnlyCursor -> ..., value
    /// 
    /// Reads the nth column of the row pointed at by the cursor and pushes it onto the stack
    /// </summary>
    public class ColumnOperation : IOperation
    {
        public int ColumnIndex { get; }

        public ColumnOperation(int columnIndex)
        {
            ColumnIndex = columnIndex;
        }

        public override string ToString()
        {
            return $"COL {ColumnIndex}";
        }
    }
}
