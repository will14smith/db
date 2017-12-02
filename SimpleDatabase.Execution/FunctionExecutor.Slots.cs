using SimpleDatabase.Execution.Operations.Slots;
using SimpleDatabase.Execution.Values;

namespace SimpleDatabase.Execution
{
    public partial class FunctionExecutor
    {
        private static (FunctionState, Result) Execute(FunctionState state, LoadOperation loadOperation)
        {
            var value = state.GetSlot(loadOperation.Slot);

            state = state.PushValue(value);

            return (state, new Result.Next());
        }

        private static (FunctionState, Result) Execute(FunctionState state, StoreOperation storeOperation)
        {
            Value value;
            (state, value) = state.PopValue();

            state = state.SetSlot(storeOperation.Slot, value);

            return (state, new Result.Next());
        }
    }
}
