using SimpleDatabase.Execution;
using SimpleDatabase.Execution.Operations.Slots;

namespace SimpleDatabase.Planning.Items
{
    public class SlotItem : Item
    {
        private readonly SlotLabel _slot;

        public SlotItem(IOperationGenerator generator, SlotLabel slot) : base(generator)
        {
            _slot = slot;
        }

        public override Item Load()
        {
            Generator.Emit(new LoadOperation(_slot));

            // TODO type
            return new StackItem(Generator);
        }

        public override void Store()
        {
            Generator.Emit(new StoreOperation(_slot));
        }
    }

    public class StackItem : Item
    {
        public StackItem(IOperationGenerator generator) : base(generator)
        {
        }

        public override Item Load()
        {
            return this;
        }
    }
}
