namespace SimpleDatabase.Execution.Operations.Columns
{
    /// <summary>
    /// ..., T : ReadOnlyCursor -> ..., key
    /// 
    /// Reads the key of the row pointed at by the cursor and pushes it onto the stack
    /// </summary>
    public class KeyOperation : IOperation
    {
        public override string ToString()
        {
            return "KEY";
        }
    }
}
