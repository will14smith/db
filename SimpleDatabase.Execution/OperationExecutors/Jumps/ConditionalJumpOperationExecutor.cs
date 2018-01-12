using System;
using SimpleDatabase.Execution.Operations.Jumps;
using SimpleDatabase.Execution.Values;

namespace SimpleDatabase.Execution.OperationExecutors.Jumps
{
    public class ConditionalJumpOperationExecutor : IOperationExecutor<ConditionalJumpOperation>
    {
        public (FunctionState, OperationResult) Execute(FunctionState state, ConditionalJumpOperation operation)
        {
            Value value1, value2;
            (state, value1) = state.PopValue();
            (state, value2) = state.PopValue();

            if (Compare(operation.Comparison, value1, value2))
            {
                return (state, new OperationResult.Jump(operation.Address));
            }

            return (state, new OperationResult.Next());
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
