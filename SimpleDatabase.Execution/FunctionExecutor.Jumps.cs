using System;
using SimpleDatabase.Execution.Operations.Jumps;
using SimpleDatabase.Execution.Values;

namespace SimpleDatabase.Execution
{
    public partial class FunctionExecutor
    {
        private (FunctionState, Result) Execute(FunctionState state, ConditionalJumpOperation conditionalJumpOperation)
        {
            Value value1, value2;
            (state, value1) = state.PopValue();
            (state, value2) = state.PopValue();

            if (Compare(conditionalJumpOperation.Comparison, value1, value2))
            {
                return (state, new Result.Jump(conditionalJumpOperation.Address));
            }

            return (state, new Result.Next());
        }

        private static (FunctionState, Result) Execute(FunctionState state, JumpOperation jumpOperation)
        {
            return (state, new Result.Jump(jumpOperation.Address));
        }

        private bool Compare(Comparison comparison, Value value1, Value value2)
        {
            if (!(value1 is ObjectValue o1) || !(value2 is ObjectValue o2))
                throw new NotImplementedException();

            switch (comparison)
            {
                case Comparison.Equal:
                    return Equals(o1.Value, o2.Value);
                case Comparison.NotEqual:
                    return !Equals(o1.Value, o2.Value);
                default:
                    throw new ArgumentOutOfRangeException(nameof(comparison), comparison, null);
            }
        }
    }
}
