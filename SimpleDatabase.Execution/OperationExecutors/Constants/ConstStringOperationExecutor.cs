using SimpleDatabase.Execution.Operations.Constants;
using SimpleDatabase.Execution.Values;

namespace SimpleDatabase.Execution.OperationExecutors.Constants
{
    public class ConstStringOperationExecutor : IOperationExecutor<ConstStringOperation>
    {
        public (FunctionState, OperationResult) Execute(FunctionState state, ConstStringOperation operation)
        {
            state = state.PushValue(new ObjectValue(operation.Value));

            return (state, new OperationResult.Next());
        }
    }
}