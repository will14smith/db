using System;
using System.Threading;

namespace SimpleDatabase.Execution
{
    public struct SlotLabel : IEquatable<SlotLabel>
    {
        private static int _counter;

        private readonly int _id;

        public SlotLabel(int id) : this()
        {
            _id = id;
        }

        public static SlotLabel Create()
        {
            return new SlotLabel(Interlocked.Increment(ref _counter));
        }

        public override string ToString()
        {
            return $"Slot {_id}";
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is SlotLabel label && Equals(label);
        }

        public bool Equals(SlotLabel other)
        {
            return _id == other._id;
        }

        public override int GetHashCode()
        {
            return _id;
        }

        public static bool operator ==(SlotLabel left, SlotLabel right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SlotLabel left, SlotLabel right)
        {
            return !left.Equals(right);
        }
    }
}
