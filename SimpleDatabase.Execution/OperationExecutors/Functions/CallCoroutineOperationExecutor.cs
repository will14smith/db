using SimpleDatabase.Execution.Operations.Functions;
using SimpleDatabase.Execution.Transactions;
using SimpleDatabase.Execution.Values;
using SimpleDatabase.Storage;
using SimpleDatabase.Storage.Paging;

namespace SimpleDatabase.Execution.OperationExecutors.Functions
{
    public class CallCoroutineOperationExecutor : IOperationExecutor<CallCoroutineOperation>
    {
        private readonly DatabaseManager _databaseManager;
        private readonly ITransactionManager _txm;
        private readonly Program _program;

        public CallCoroutineOperationExecutor(DatabaseManager databaseManager, ITransactionManager txm, Program program)
        {
            _databaseManager = databaseManager;
            _txm = txm;
            _program = program;
        }

        public (FunctionState, OperationResult) Execute(FunctionState state, CallCoroutineOperation operation)
        {
            CoroutineHandle handle;
            (state, handle) = state.PopValue<CoroutineHandle>();

            var function = _program.Functions[handle.Function];

            var coroutineExecutor = new FunctionExecutor(_databaseManager, _txm, _program, function, handle.Args);
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
