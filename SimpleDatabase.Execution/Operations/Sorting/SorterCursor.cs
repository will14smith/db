namespace SimpleDatabase.Execution.Operations.Sorting
{
    /// <summary>
    /// ..., S -> ..., ReadOnlyCursor
    /// 
    /// Pops the sorter from the stack.
    /// Creates a cursor for it and pushes it to the stack.
    /// </summary>
    public class SorterCursor : IOperation
    {
        public override string ToString()
        {
            return "SORTER.CUR";
        }
    }
}
