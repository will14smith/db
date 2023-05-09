using SimpleDatabase.Execution.Operations.Cursors;
using SimpleDatabase.Execution.Transactions;
using SimpleDatabase.Execution.Values;
using SimpleDatabase.Storage;
using SimpleDatabase.Storage.Paging;

namespace SimpleDatabase.Execution.OperationExecutors.Cursors
{
    public class OpenReadTableOperationExecutor : IOperationExecutor<OpenReadTableOperation>
    {
        private readonly DatabaseManager _databaseManager;
        private readonly ITransactionManager _txm;

        public OpenReadTableOperationExecutor(DatabaseManager databaseManager, ITransactionManager txm)
        {
            _databaseManager = databaseManager;
            _txm = txm;
        }

        public (FunctionState, OperationResult) Execute(FunctionState state, OpenReadTableOperation operation)
        {
            // TODO acquire read lock
            var table = operation.Table;
            var tableManager = _databaseManager.GetTableManagerFor(table);

            var tableCursor = new HeapCursor(tableManager, _txm, false);

            var cursor = new CursorValue(false);
            cursor = cursor.SetNextCursor(tableCursor);

            state = state.PushValue(cursor);
            return (state, new OperationResult.Next());
        }
    }
}
