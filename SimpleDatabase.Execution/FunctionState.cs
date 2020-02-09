using System;
using System.Collections.Generic;
using System.Linq;
using SimpleDatabase.Execution.Values;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Execution
{
    public class FunctionState
    {
        // TODO make this class immutable
        private int _pc;
        private readonly Stack<Value> _stack = new Stack<Value>();
        private readonly IDictionary<SlotLabel, Value> _slots = new Dictionary<SlotLabel, Value>();

        public int StackCount => _stack.Count;

        public FunctionState(IReadOnlyDictionary<SlotLabel, SlotDefinition> slotDefinitions)
        {
            _pc = 0;

            foreach (var kvp in slotDefinitions)
            {
                _slots.Add(kvp.Key, new NullValue());
            }
        }

        public int GetPC()
        {
            return _pc;
        }
        public FunctionState SetPC(int pc)
        {
            if (pc < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pc), "pc must not be negative");
            }

            _pc = pc;

            return this;
        }
        
        public (FunctionState, Value) PopValue()
        {
            return (this, _stack.Pop());
        }
        public FunctionState PushValue(Value value)
        {
            _stack.Push(value);
            return this;
        }

        public Value GetSlot(SlotLabel label)
        {
            return _slots[label];
        }
        public FunctionState SetSlot(SlotLabel label, Value value)
        {
            _slots[label] = value;

            return this;
        }

        private sealed class PCStackSlotsEqualityComparer : IEqualityComparer<FunctionState>
        {
            public bool Equals(FunctionState x, FunctionState y)
            {
                if (ReferenceEquals(x, y)) return true;

                if (x is null) return false;
                if (y is null) return false;

                if (x._pc != y._pc) return false;
                if (!x._stack.SequenceEqual(y._stack)) return false;
                if (!new DictionaryComparer<SlotLabel, Value>().Equals(x._slots, y._slots)) return false;
                
                return true;
            }

            public int GetHashCode(FunctionState obj)
            {
                throw new NotSupportedException();
            }
        }

        public static IEqualityComparer<FunctionState> EqualityComparer { get; } = new PCStackSlotsEqualityComparer();
    }
}
