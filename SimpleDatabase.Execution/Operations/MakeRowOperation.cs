namespace SimpleDatabase.Execution.Operations
{
    /// <summary>
    /// ..., c1, c2, ..., cN -> ..., R
    /// 
    /// Pops N columns from the stack and creates a row object
    /// Note that the column order is the opposite of the pop order
    /// </summary>
    public class MakeRowOperation : IOperation
    {
        public int ColumnCount { get; }

        public MakeRowOperation(int columnCount)
        {
            ColumnCount = columnCount;
        }

        public override string ToString()
        {
            return $"MK.ROW {ColumnCount}";
        }
    }
}
