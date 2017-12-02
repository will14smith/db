using SimpleDatabase.Execution.Operations.Constants;
using SimpleDatabase.Execution.Values;

namespace SimpleDatabase.Execution
{
    public partial class FunctionExecutor
    {
        private static (FunctionState, Result) Execute(FunctionState state, ConstIntOperation constIntOperation)
        {
            state = state.PushValue(new ObjectValue(constIntOperation.Value));

            return (state, new Result.Next());
        }

        private static (FunctionState, Result) Execute(FunctionState state, ConstStringOperation constStringOperation)
        {
            state = state.PushValue(new ObjectValue(constStringOperation.Value));

            return (state, new Result.Next());
        }
    }
}
