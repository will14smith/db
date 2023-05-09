using SimpleDatabase.Execution.Operations.Cursors;
using SimpleDatabase.Execution.Transactions;
using SimpleDatabase.Execution.Values;
using SimpleDatabase.Storage;
using SimpleDatabase.Storage.Paging;

namespace SimpleDatabase.Execution.OperationExecutors.Cursors
{
    public class OpenReadIndexOperationExecutor : IOperationExecutor<OpenReadIndexOperation>
    {
        private readonly DatabaseManager _databaseManager;
        private readonly ITransactionManager _txm;

        public OpenReadIndexOperationExecutor(DatabaseManager databaseManager, ITransactionManager txm)
        {
            _databaseManager = databaseManager;
            _txm = txm;
        }

        public (FunctionState, OperationResult) Execute(FunctionState state, OpenReadIndexOperation operation)
        {
            // TODO aquire read lock
            var table = operation.Table;
            var tableManager = _databaseManager.GetTableManagerFor(table);
            
            var index = operation.Index;
            
            var tableCursor = new IndexCursor(tableManager, _txm, index, false);

            var cursor = new CursorValue(false);
            cursor = cursor.SetNextCursor(tableCursor);

            state = state.PushValue(cursor);
            return (state, new OperationResult.Next());
        }
    }
}
