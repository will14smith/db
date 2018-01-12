using SimpleDatabase.Execution.Operations.Functions;
using SimpleDatabase.Execution.Values;

namespace SimpleDatabase.Execution.OperationExecutors.Functions
{
    public class ReturnOperationExecutor : IOperationExecutor<ReturnOperation>
    {
        public (FunctionState, OperationResult) Execute(FunctionState state, ReturnOperation operation)
        {
            Value value;
            (state, value) = state.PopValue();

            return (state, new OperationResult.Yield(new OperationResult.Next(), value));
        }
    }
}