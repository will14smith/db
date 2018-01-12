using SimpleDatabase.Execution.Operations;

namespace SimpleDatabase.Execution.OperationExecutors
{
    public interface IOperationExecutor<in TOperation> 
        where TOperation : IOperation
    {
        (FunctionState, OperationResult) Execute(FunctionState state, TOperation operation);
    }
}
