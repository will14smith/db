namespace SimpleDatabase.Core.Execution.Operations
{
    /// <summary>
    /// ..., c1, c2, ..., cN -> ...
    /// 
    /// Pops N columns from the stack and yields them as a row result
    /// Note that the column order is the opposite of the pop order
    /// </summary>
    public class YieldRowOperation : Operation
    {
        public int ColumnCount { get; }

        public YieldRowOperation(int columnCount)
        {
            ColumnCount = columnCount;
        }
    }
}
