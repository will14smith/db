using SimpleDatabase.Execution.Operations.Slots;

namespace SimpleDatabase.Execution.OperationExecutors.Slots
{
    public class LoadOperationExecutor : IOperationExecutor<LoadOperation>
    {
        public (FunctionState, OperationResult) Execute(FunctionState state, LoadOperation operation)
        {
            var value = state.GetSlot(operation.Slot);

            state = state.PushValue(value);

            return (state, new OperationResult.Next());
        }
    }
}
