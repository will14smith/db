namespace SimpleDatabase.Execution.Operations.Slots
{
    /// <summary>
    /// ... -> ..., x
    /// 
    /// Reads the value in the slot and pushes on to the stack
    /// </summary>
    public class LoadOperation : Operation
    {
        public int Slot { get; }

        public LoadOperation(int slot)
        {
            Slot = slot;
        }
    }
}
