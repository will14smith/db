using SimpleDatabase.Execution.Operations.Slots;
using SimpleDatabase.Execution.Values;

namespace SimpleDatabase.Execution.OperationExecutors.Slots
{
    public class StoreOperationExecutor : IOperationExecutor<StoreOperation>
    {
        public (FunctionState, OperationResult) Execute(FunctionState state, StoreOperation operation)
        {
            Value? value;
            (state, value) = state.PopValue();

            state = state.SetSlot(operation.Slot, value);

            return (state, new OperationResult.Next());
        }
    }
}
