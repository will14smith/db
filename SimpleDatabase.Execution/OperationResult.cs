using System;
using SimpleDatabase.Execution.Values;

namespace SimpleDatabase.Execution
{
    public abstract class OperationResult
    {
        public class Next : OperationResult { }
        public class Jump : OperationResult
        {
            public ProgramLabel Address { get; }

            public Jump(ProgramLabel address)
            {
                Address = address;
            }
        }
        public class Finished : OperationResult { }

        public class Yield : OperationResult
        {
            public OperationResult Inner { get; }
            public Value? Value { get; }

            public Yield(OperationResult inner, Value? value)
            {
                if (inner is Yield)
                {
                    throw new InvalidOperationException("Cannot have recursive yields");
                }

                Inner = inner;
                Value = value;
            }
        }
    }
}