using System.Collections.Generic;
using SimpleDatabase.Execution.Operations;

namespace SimpleDatabase.Execution
{
    public class Program
    {
        public IReadOnlyList<Operation> Operations { get; }
        public IReadOnlyList<SlotDefinition> Slots { get; }

        public Program(IReadOnlyList<Operation> operations, IReadOnlyList<SlotDefinition> slots)
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
