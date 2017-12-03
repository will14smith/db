using SimpleDatabase.Execution;
using SimpleDatabase.Execution.Operations.Slots;

namespace SimpleDatabase.Planning.Items
{
    public class SlotItem : Item
    {
        private readonly SlotLabel _slot;

        public SlotItem(SlotLabel slot)
        {
            _slot = slot;
        }

        public override Item Load(IOperationGenerator generator)
        {
            generator.Emit(new LoadOperation(_slot));

            // TODO type
            return new StackItem();
        }

        public override void Store(IOperationGenerator generator)
        {
            generator.Emit(new StoreOperation(_slot));
        }
    }
}
