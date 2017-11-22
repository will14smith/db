namespace SimpleDatabase.Core.Execution.Operations
{
    /// <summary>
    /// ..., T1 : ReadOnlyCursor -> ..., T2
    /// 
    /// Creates a cursor T2 pointing to the first entry in the tree of T1 and configures it for forward movement
    /// If the table is empty then it jumps to the empty address, otherwise it moves to the next instruction
    /// </summary>
    public class FirstOperation : Operation
    {
        public int EmptyAddress { get; }

        public FirstOperation(int emptyAddress)
        {
            EmptyAddress = emptyAddress;
        }
    }
}
