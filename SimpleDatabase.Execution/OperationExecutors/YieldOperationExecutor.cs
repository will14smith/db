using SimpleDatabase.Execution.Operations;
using SimpleDatabase.Execution.Values;

namespace SimpleDatabase.Execution.OperationExecutors
{
    public class YieldOperationExecutor : IOperationExecutor<YieldOperation>
    {
        public (FunctionState, OperationResult) Execute(FunctionState state, YieldOperation operation)
        {
            Value? value;
            (state, value) = state.PopValue();

            return (state, new OperationResult.Yield(new OperationResult.Next(), value));
        }
    }
}