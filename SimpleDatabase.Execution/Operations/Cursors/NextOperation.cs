namespace SimpleDatabase.Execution.Operations.Cursors
{
    /// <summary>
    /// ..., T1 : ReadOnlyCursor -> ..., T2
    /// 
    /// Creates a cursor T2 pointing to the next entry after T1
    /// If there is a next entry it jumps to successAddress, otherwise it moves to the next instruction
    /// </summary>
    public class NextOperation : IOperation
    {
        public ProgramLabel SuccessAddress { get; }

        public NextOperation(ProgramLabel successAddress)
        {
            SuccessAddress = successAddress;
        }
    }
}
