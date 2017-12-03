namespace SimpleDatabase.Execution.Operations.Cursors
{
    /// <summary>
    /// ..., WritableCursor -> ..., WritableCursor
    /// 
    /// Pops a cursor off the stack.
    /// Deletes the element the cursor is pointing at.
    /// Pushes a new cursor onto the stack pointing at the previous element.
    /// TODO first?
    /// </summary>
    public class DeleteOperation : IOperation
    {
        public override string ToString()
        {
            return "DELETE";
        }
    }
}
