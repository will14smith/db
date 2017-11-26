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
        private readonly Value[] _slots;

        public int StackCount => _stack.Count;

        public ProgramState(int slotCount)
        {
            if(slotCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(slotCount), "slotCount must not be negative");
            }

            _pc = 0;
            _slots = new Value[slotCount];
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

        public Value GetSlot(int index)
        {
            if (index < 0 || index >= _slots.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "index must be a valid index in the slots");
            }

            return _slots[index];
        }
        public ProgramState SetSlot(int index, Value value)
        {
            if (index < 0 || index >= _slots.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "index must be a valid index in the slots");
            }

            _slots[index] = value;

            return this;
        }
    }
}
