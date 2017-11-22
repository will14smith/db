using SimpleDatabase.Core.Execution.Operations;
using System.Collections.Generic;

namespace SimpleDatabase.Core.Execution
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
