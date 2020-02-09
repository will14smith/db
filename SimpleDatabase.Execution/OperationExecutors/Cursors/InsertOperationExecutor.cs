using System;
using System.Linq;
using SimpleDatabase.Execution.Operations.Cursors;
using SimpleDatabase.Execution.Transactions;
using SimpleDatabase.Execution.Values;
using SimpleDatabase.Schemas;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Execution.OperationExecutors.Cursors
{
    public class InsertOperationExecutor : IOperationExecutor<InsertOperation>
    {
        private readonly ITransactionManager _txm;

        public InsertOperationExecutor(ITransactionManager txm)
        {
            _txm = txm;
        }

        public (FunctionState, OperationResult) Execute(FunctionState state, InsertOperation operation)
        {
            var tx = _txm.Current;
            if(tx is null) throw new InvalidOperationException("Cannot insert outside a transaction");
            
            RowValue row;
            Value targetValue;

            (state, targetValue) = state.PopValue();
            (state, row) = state.PopValue<RowValue>();

            IInsertTarget insertTarget;
            switch (targetValue)
            {
                case CursorValue cursorValue:
                {
                    var cursor = cursorValue.Cursor.OrElse(() =>
                    {
                        return cursorValue.NextCursor.OrElse(() => throw new InvalidOperationException("Something is right dodgy with this cursor..."));
                    });

                    insertTarget = (IInsertTarget)cursor;
                }
                    break;
                case IInsertTarget target:
                    insertTarget = target;
                    break;

                default:
                    throw new NotImplementedException($"Unsupported type: {targetValue.GetType()}");
            }

            var values = row.Values.Cast<ObjectValue>().Select(x => new ColumnValue(x.Value)).ToList();
            var insertableRow = new Row(values, tx.Id, TransactionId.None());

            var insertResult = insertTarget.Insert(insertableRow);
            switch (insertResult)
            {
                case InsertTargetResult.Success _:
                    return (state, new OperationResult.Next());

                default:
                    throw new NotImplementedException($"Unsupported type: {insertResult.GetType().Name}");
            }
        }
    }
}
