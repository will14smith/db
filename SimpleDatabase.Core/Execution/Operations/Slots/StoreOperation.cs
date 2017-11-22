namespace SimpleDatabase.Core.Execution.Operations
{
    /// <summary>
    /// ..., x -> ...
    /// 
    /// Pops a value off the stack and stores it in the slot
    /// </summary>
    public class StoreOperation : Operation
    {
        public int Slot { get; }

        public StoreOperation(int slot)
        {
            Slot = slot;
        }
    }
}
