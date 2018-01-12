using SimpleDatabase.Execution.Operations;

namespace SimpleDatabase.Execution.OperationExecutors
{
    public class FinishOperationExecutor : IOperationExecutor<FinishOperation>
    {
        public (FunctionState, OperationResult) Execute(FunctionState state, FinishOperation operation)
        {
            return (state, new OperationResult.Finished());
        }
    }
}