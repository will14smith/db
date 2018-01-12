using SimpleDatabase.Execution.Operations.Jumps;

namespace SimpleDatabase.Execution.OperationExecutors.Jumps
{
    public class JumpOperationExecutor : IOperationExecutor<JumpOperation>
    {
        public (FunctionState, OperationResult) Execute(FunctionState state, JumpOperation operation)
        {
            return (state, new OperationResult.Jump(operation.Address));
        }
    }
}