namespace SimpleDatabase.Execution.Operations.Slots
{
    /// <summary>
    /// ..., x -> ...
    /// 
    /// Pops a value off the stack and stores it in the slot
    /// </summary>
    public class StoreOperation : IOperation
    {
        public SlotLabel Slot { get; }

        public StoreOperation(SlotLabel slot)
        {
            Slot = slot;
        }

        public override string ToString()
        {
            return $"STORE {Slot}";
        }
    }
}
