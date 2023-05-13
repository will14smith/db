using System;
using System.Threading;

namespace SimpleDatabase.Execution
{
    public struct SlotLabel : IEquatable<SlotLabel>
    {
        private readonly int _id;
        private readonly string _name;

        public SlotLabel(int id, string? name) : this()
        {
            _id = id;
            _name = name?.Replace(" ", "_") ?? "";
        }
        
        public override string ToString()
        {
            if (_name == "")
            {
                return $"S{_id}";
            }

            return $"S{_id}_{_name}";
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
