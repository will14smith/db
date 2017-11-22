namespace SimpleDatabase.Core.Execution.Operations
{
    /// <summary>
    /// ..., T1 : ReadOnlyCursor -> ..., T2
    /// 
    /// Creates a cursor T2 pointing to the next entry after T1
    /// If there is a next entry it jumps to successAddress, otherwise it moves to the next instruction
    /// </summary>
    public class NextOperation : Operation
    {
        public int SuccessAddress { get; }

        public NextOperation(int successAddress)
        {
            SuccessAddress = successAddress;
        }
    }
}
