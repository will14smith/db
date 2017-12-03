using System;
using System.Collections.Generic;
using SimpleDatabase.Execution;
using SimpleDatabase.Execution.Operations;

namespace SimpleDatabase.Planning
{
    internal class OperationGenerator : IOperationGenerator
    {
        private Dictionary<SlotLabel, SlotDefinition> _slots = new Dictionary<SlotLabel, SlotDefinition>();
        private List<IOperation> _operations = new List<IOperation>();

        public FunctionLabel NewFunction(Function function)
        {
            throw new NotImplementedException();
        }

        public SlotLabel NewSlot(SlotDefinition definition)
        {
            var slot = SlotLabel.Create();
            _slots.Add(slot, definition);

            return slot;
        }

        public ProgramLabel NewLabel()
        {
            // TODO track it
            return ProgramLabel.Create();
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

        public Program CreateProgram()
        {
            var main = FunctionLabel.Create();
            var functions = new Dictionary<FunctionLabel, Function>
            {
                { main, new Function(_operations, _slots) }
            };

            return new Program(main, functions);
        }
    }
}