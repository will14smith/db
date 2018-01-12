using SimpleDatabase.Execution.Operations.Constants;
using SimpleDatabase.Execution.Values;

namespace SimpleDatabase.Execution.OperationExecutors.Constants
{
    public class ConstIntOperationExecutor : IOperationExecutor<ConstIntOperation>
    {
        public (FunctionState, OperationResult) Execute(FunctionState state, ConstIntOperation operation)
        {
            state = state.PushValue(new ObjectValue(operation.Value));

            return (state, new OperationResult.Next());
        }
    }
}
