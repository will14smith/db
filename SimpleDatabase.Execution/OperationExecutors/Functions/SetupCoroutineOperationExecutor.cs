using System.Collections.Generic;
using SimpleDatabase.Execution.Operations.Functions;
using SimpleDatabase.Execution.Values;

namespace SimpleDatabase.Execution.OperationExecutors.Functions
{
    public class SetupCoroutineOperationExecutor : IOperationExecutor<SetupCoroutineOperation>
    {
        private readonly IReadOnlyDictionary<FunctionLabel, Function> _functions;

        public SetupCoroutineOperationExecutor(IReadOnlyDictionary<FunctionLabel, Function> functions)
        {
            _functions = functions;
        }

        public (FunctionState, OperationResult) Execute(FunctionState state, SetupCoroutineOperation operation)
        {
            var args = new Value[operation.ArgumentCount];

            for (var i = operation.ArgumentCount - 1; i >= 0; i--)
            {
                (state, args[i]) = state.PopValue();
            }

            var function = _functions[operation.Function];

            var handle = new CoroutineHandle(operation.Function, args, new FunctionState(function.Slots));

            state = state.PushValue(handle);

            return (state, new OperationResult.Next());
        }
    }
}