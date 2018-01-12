using SimpleDatabase.Execution.Operations.Functions;
using SimpleDatabase.Execution.Transactions;
using SimpleDatabase.Execution.Values;
using SimpleDatabase.Storage.Paging;

namespace SimpleDatabase.Execution.OperationExecutors.Functions
{
    public class CallCoroutineOperationExecutor : IOperationExecutor<CallCoroutineOperation>
    {
        private readonly IPager _pager;
        private readonly ITransactionManager _txm;
        private readonly Program _program;

        public CallCoroutineOperationExecutor(IPager pager, ITransactionManager txm, Program program)
        {
            _pager = pager;
            _txm = txm;
            _program = program;
        }

        public (FunctionState, OperationResult) Execute(FunctionState state, CallCoroutineOperation operation)
        {
            CoroutineHandle handle;
            (state, handle) = state.PopValue<CoroutineHandle>();

            var function = _program.Functions[handle.Function];

            var coroutineExecutor = new FunctionExecutor(_pager, _txm, _program, function, handle.Args);
            var (coroutineState, coroutineValue) = coroutineExecutor.ExecuteStep(handle.State);

            handle.SetState(coroutineState);

            if (coroutineValue == null)
            {
                return (state, new OperationResult.Jump(operation.Done));
            }

            state = state.PushValue(coroutineValue);

            return (state, new OperationResult.Next());
        }
    }
}
