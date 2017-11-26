namespace SimpleDatabase.Execution.Operations.Slots
{
    /// <summary>
    /// ... -> ..., x
    /// 
    /// Reads the value in the slot and pushes on to the stack
    /// </summary>
    public class LoadOperation : IOperation
    {
        public SlotLabel Slot { get; }

        public LoadOperation(SlotLabel slot)
        {
            Slot = slot;
        }
    }
}
