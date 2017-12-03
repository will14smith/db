namespace SimpleDatabase.Execution.Operations.Cursors
{
    /// <summary>
    /// ..., R, WritableCursor -> ...
    /// 
    /// Pops a row and cursor off the stack.
    /// Inserts the row into the cursor's table.
    /// </summary>
    public class InsertOperation : IOperation
    {
        public override string ToString()
        {
            return "INSERT";
        }
    }
}
