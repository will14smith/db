using System.Collections.Generic;
using SimpleDatabase.Execution.Operations;

namespace SimpleDatabase.Execution
{
    public class Program
    {
        public IReadOnlyList<IOperation> Operations { get; }
        public IReadOnlyDictionary<SlotLabel, SlotDefinition> Slots { get; }

        public Program(IReadOnlyList<IOperation> operations, IReadOnlyDictionary<SlotLabel, SlotDefinition> slots)
        {
            Operations = operations;
            Slots = slots;
        }
    }

    public class SlotDefinition
    {
        // TODO type
    }
}
