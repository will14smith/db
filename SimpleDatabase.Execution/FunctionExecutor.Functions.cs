using SimpleDatabase.Execution.Operations.Functions;
using SimpleDatabase.Execution.Values;

namespace SimpleDatabase.Execution
{
    public partial class FunctionExecutor
    {
        private static (FunctionState, Result) Execute(FunctionState state, ReturnOperation ret)
        {
            Value value;
            (state, value) = state.PopValue();

            return (state, new Result.Yield(new Result.Next(), value));
        }

        private (FunctionState, Result) Execute(FunctionState state, SetupCoroutineOperation setupCoroutineOperation)
        {
            var args = new Value[setupCoroutineOperation.ArgumentCount];

            for (var i = setupCoroutineOperation.ArgumentCount - 1; i >= 0; i--)
            {
                (state, args[i]) = state.PopValue();
            }

            var function = _program.Functions[setupCoroutineOperation.Function];

            var handle = new CoroutineHandle(setupCoroutineOperation.Function, args, new FunctionState(function.Slots));

            state = state.PushValue(handle);

            return (state, new Result.Next());
        }

        private (FunctionState, Result) Execute(FunctionState state, CallCoroutineOperation callCoroutineOperation)
        {
            CoroutineHandle handle;
            (state, handle) = state.PopValue<CoroutineHandle>();

            var function = _program.Functions[handle.Function];

            var coroutineExecutor = new FunctionExecutor(function, handle.Args, _pager, _txm, _program);
            var (coroutineState, coroutineValue) = coroutineExecutor.ExecuteStep(handle.State);

            handle.SetState(coroutineState);

            if (coroutineValue == null)
            {
                return (state, new Result.Jump(callCoroutineOperation.Done));
            }

            state = state.PushValue(coroutineValue);

            return (state, new Result.Next());
        }
    }
}
