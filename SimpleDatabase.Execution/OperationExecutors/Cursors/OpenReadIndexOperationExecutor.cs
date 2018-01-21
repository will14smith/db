using SimpleDatabase.Execution.Operations.Cursors;
using SimpleDatabase.Execution.Transactions;
using SimpleDatabase.Execution.Values;
using SimpleDatabase.Storage.Paging;

namespace SimpleDatabase.Execution.OperationExecutors.Cursors
{
    public class OpenReadIndexOperationExecutor : IOperationExecutor<OpenReadIndexOperation>
    {
        private readonly IPager _pager;
        private readonly ITransactionManager _txm;

        public OpenReadIndexOperationExecutor(IPager pager, ITransactionManager txm)
        {
            _pager = pager;
            _txm = txm;
        }

        public (FunctionState, OperationResult) Execute(FunctionState state, OpenReadIndexOperation operation)
        {
            // TODO aquire read lock
            var table = operation.Table;
            var index = operation.Index;
            var tableCursor = new IndexCursor(_pager, _txm, table, index, false);

            var cursor = new CursorValue(false);
            cursor = cursor.SetNextCursor(tableCursor);

            state = state.PushValue(cursor);
            return (state, new OperationResult.Next());
        }
    }
}
