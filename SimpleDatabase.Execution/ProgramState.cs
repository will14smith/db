using System;
using System.Collections.Generic;
using SimpleDatabase.Execution.Values;

namespace SimpleDatabase.Execution
{
    public class ProgramState
    {
        // TODO make this class immutable
        private int _pc;
        private readonly Stack<Value> _stack = new Stack<Value>();
        private readonly IDictionary<SlotLabel, Value> _slots = new Dictionary<SlotLabel, Value>();

        public int StackCount => _stack.Count;

        public ProgramState(IReadOnlyDictionary<SlotLabel, SlotDefinition> slotDefinitions)
        {
            _pc = 0;

            foreach (var (label, _) in slotDefinitions)
            {
                _slots.Add(label, null);
            }
        }


        public int GetPC()
        {
            return _pc;
        }
        public ProgramState SetPC(int pc)
        {
            if (pc < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pc), "pc must not be negative");
            }

            _pc = pc;

            return this;
        }


        public (ProgramState, Value) PopValue()
        {
            return (this, _stack.Pop());
        }
        public ProgramState PushValue(Value value)
        {
            _stack.Push(value);
            return this;
        }

        public Value GetSlot(SlotLabel label)
        {
            return _slots[label];
        }
        public ProgramState SetSlot(SlotLabel label, Value value)
        {
            _slots[label] = value;

            return this;
        }
    }
}
