using System;
using System.Threading;
using SimpleDatabase.Execution.Operations;

namespace SimpleDatabase.Execution
{
    public struct FunctionLabel : IOperation, IEquatable<FunctionLabel>
    {
        private static int _counter;

        private readonly int _id;

        public FunctionLabel(int id) : this()
        {
            _id = id;
        }

        public static FunctionLabel Create()
        {
            return new FunctionLabel(Interlocked.Increment(ref _counter));
        }

        public override string ToString()
        {
            return $"C{_id}";
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is FunctionLabel label && Equals(label);
        }

        public bool Equals(FunctionLabel other)
        {
            return _id == other._id;
        }

        public override int GetHashCode()
        {
            return _id;
        }

        public static bool operator ==(FunctionLabel left, FunctionLabel right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(FunctionLabel left, FunctionLabel right)
        {
            return !left.Equals(right);
        }
    }
}
