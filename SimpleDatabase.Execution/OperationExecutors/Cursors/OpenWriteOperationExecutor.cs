using SimpleDatabase.Execution.Operations.Cursors;
using SimpleDatabase.Execution.Transactions;
using SimpleDatabase.Execution.Values;
using SimpleDatabase.Storage.Paging;

namespace SimpleDatabase.Execution.OperationExecutors.Cursors
{
    public class OpenWriteOperationExecutor : IOperationExecutor<OpenWriteOperation>
    {
        private readonly IPager _pager;
        private readonly ITransactionManager _txm;
        private readonly IRowSerializerFactory _rowSerializerFactory;

        public OpenWriteOperationExecutor(IPager pager, ITransactionManager txm, IRowSerializerFactory rowSerializerFactory)
        {
            _pager = pager;
            _txm = txm;
            _rowSerializerFactory = rowSerializerFactory;
        }

        public (FunctionState, OperationResult) Execute(FunctionState state, OpenWriteOperation operation)
        {
            // TODO aquire write lock
            var table = operation.Table;
            var heapCursor = new HeapCursor(_pager, _txm, _rowSerializerFactory.Create(table), table, true);

            var cursor = new CursorValue(true);
            cursor = cursor.SetNextCursor(heapCursor);

            state = state.PushValue(cursor);
            return (state, new OperationResult.Next());
        }
    }
}
