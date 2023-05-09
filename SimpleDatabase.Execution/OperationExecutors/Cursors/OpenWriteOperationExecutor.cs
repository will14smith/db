using SimpleDatabase.Execution.Operations.Cursors;
using SimpleDatabase.Execution.Transactions;
using SimpleDatabase.Execution.Values;
using SimpleDatabase.Storage;
using SimpleDatabase.Storage.Paging;

namespace SimpleDatabase.Execution.OperationExecutors.Cursors
{
    public class OpenWriteOperationExecutor : IOperationExecutor<OpenWriteOperation>
    {
        private readonly DatabaseManager _databaseManager;
        private readonly ITransactionManager _txm;

        public OpenWriteOperationExecutor(DatabaseManager databaseManager, ITransactionManager txm)
        {
            _databaseManager = databaseManager;
            _txm = txm;
        }

        public (FunctionState, OperationResult) Execute(FunctionState state, OpenWriteOperation operation)
        {
            // TODO aquire write lock
            var table = operation.Table;
            var tableManager = _databaseManager.GetTableManagerFor(table);
            
            var heapCursor = new HeapCursor(tableManager, _txm, true);

            var cursor = new CursorValue(true);
            cursor = cursor.SetNextCursor(heapCursor);

            state = state.PushValue(cursor);
            return (state, new OperationResult.Next());
        }
    }
}
