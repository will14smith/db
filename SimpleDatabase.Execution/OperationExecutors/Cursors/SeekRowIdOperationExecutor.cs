using System;
using SimpleDatabase.Execution.Operations.Cursors;
using SimpleDatabase.Execution.Transactions;
using SimpleDatabase.Execution.Values;
using SimpleDatabase.Storage;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Execution.OperationExecutors.Cursors;

public class SeekRowIdOperationExecutor : IOperationExecutor<SeekRowIdOperation>
{
    private readonly DatabaseManager _dbm;
    private readonly ITransactionManager _txm;

    public SeekRowIdOperationExecutor(DatabaseManager dbm, ITransactionManager txm)
    {
        _dbm = dbm;
        _txm = txm;
    }

    public (FunctionState, OperationResult) Execute(FunctionState state, SeekRowIdOperation operation)
    {
        (state, var cursorValue) = state.PopValue<CursorValue>(); 
        
        var rawCursor = cursorValue.NextCursor.OrElse(() =>
        {
            return cursorValue.Cursor.OrElse(() => throw new InvalidOperationException("cursor is null, has a position been set of this cursor?"));
        });
        if (rawCursor is not HeapCursor heapCursor)
        {
            throw new NotSupportedException("can only seek to a row id on a heap cursor");
        }

        (state, var rowId) = state.PopValue<ObjectValue>();

        heapCursor = heapCursor.MoveToLocation((int) (rowId.Value ?? throw new InvalidOperationException()));

        cursorValue = cursorValue.SetNextCursor(heapCursor);
        state = state.PushValue(cursorValue);

        return (state, new OperationResult.Next());
    }
}