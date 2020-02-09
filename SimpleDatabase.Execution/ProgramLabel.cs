using System;
using System.Threading;
using SimpleDatabase.Execution.Operations;

namespace SimpleDatabase.Execution
{
    public struct ProgramLabel : IOperation, IEquatable<ProgramLabel>
    {
        private static int _counter;

        private readonly int _id;
        private readonly string _name;

        public ProgramLabel(int id, string? name) : this()
        {
            _id = id;
            _name = name?.Replace(" ", "_") ?? "";
        }

        public static ProgramLabel Create(string? name = null)
        {
            return new ProgramLabel(Interlocked.Increment(ref _counter), name);
        }

        public override string ToString()
        {
            if (_name == "")
            {
                return $"L{_id}";
            }

            return $"L{_id}_{_name}";
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ProgramLabel label && Equals(label);
        }

        public bool Equals(ProgramLabel other)
        {
            return _id == other._id;
        }

        public override int GetHashCode()
        {
            return _id;
        }

        public static bool operator ==(ProgramLabel left, ProgramLabel right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ProgramLabel left, ProgramLabel right)
        {
            return !left.Equals(right);
        }
    }
}
