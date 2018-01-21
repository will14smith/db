using SimpleDatabase.Execution.Operations.Cursors;
using SimpleDatabase.Execution.Transactions;
using SimpleDatabase.Execution.Values;
using SimpleDatabase.Storage.Paging;

namespace SimpleDatabase.Execution.OperationExecutors.Cursors
{
    public class OpenReadTableOperationExecutor : IOperationExecutor<OpenReadTableOperation>
    {
        private readonly IPager _pager;
        private readonly ITransactionManager _txm;

        public OpenReadTableOperationExecutor(IPager pager, ITransactionManager txm)
        {
            _pager = pager;
            _txm = txm;
        }

        public (FunctionState, OperationResult) Execute(FunctionState state, OpenReadTableOperation operation)
        {
                // TODO acquire read lock
                var table = operation.Table;
                var tableCursor = new HeapCursor(_pager, _txm, table, false);

                var cursor = new CursorValue(false);
                cursor = cursor.SetNextCursor(tableCursor);

                state = state.PushValue(cursor);
                return (state, new OperationResult.Next());
        }
    }
}
