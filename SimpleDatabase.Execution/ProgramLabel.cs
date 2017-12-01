using System;
using System.Threading;
using SimpleDatabase.Execution.Operations;

namespace SimpleDatabase.Execution
{
    public struct ProgramLabel : IOperation, IEquatable<ProgramLabel>
    {
        private static int _counter;

        private readonly int _id;

        public ProgramLabel(int id) : this()
        {
            _id = id;
        }

        public static ProgramLabel Create()
        {
            return new ProgramLabel(Interlocked.Increment(ref _counter));
        }

        public override string ToString()
        {
            return $"L{_id}";
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
