using System.Collections.Generic;
using SimpleDatabase.Execution;
using SimpleDatabase.Execution.Operations;

namespace SimpleDatabase.Planning
{
    internal class OperationGenerator : IOperationGenerator
    {
        public FunctionLabel Label { get; }

        private readonly ProgramGenerator _programGenerator;

        private readonly Dictionary<SlotLabel, SlotDefinition> _slots = new Dictionary<SlotLabel, SlotDefinition>();
        private readonly List<IOperation> _operations = new List<IOperation>();

        public OperationGenerator(ProgramGenerator programGenerator, FunctionLabel label)
        {
            Label = label;
            _programGenerator = programGenerator;
        }

        public IOperationGenerator NewFunction()
        {
            return _programGenerator.NewFunction();
        }

        public SlotLabel NewSlot(SlotDefinition definition)
        {
            var slot = SlotLabel.Create(definition.Name);
            _slots.Add(slot, definition);

            return slot;
        }

        public ProgramLabel NewLabel(string name)
        {
            // TODO track it
            return ProgramLabel.Create(name);
        }

        public void MarkLabel(ProgramLabel label)
        {
            // TODO mark it as used (to avoid double marking...)
            _operations.Add(label);
        }

        public void Emit(IOperation operation)
        {
            if (operation is ProgramLabel label)
            {
                MarkLabel(label);
                return;
            }

            _operations.Add(operation);
        }

        public Function CreateFunction()
        {
            return new Function(_operations, _slots);
        }
    }
}