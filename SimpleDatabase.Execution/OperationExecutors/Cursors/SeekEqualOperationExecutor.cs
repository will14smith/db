using System;
using SimpleDatabase.Execution.Operations.Cursors;
using SimpleDatabase.Execution.Values;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Serialization;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Execution.OperationExecutors.Cursors;

public class SeekEqualOperationExecutor : IOperationExecutor<SeekEqualOperation>
{
    public (FunctionState, OperationResult) Execute(FunctionState state, SeekEqualOperation operation)
    {
        (state, var cursorValue) = state.PopValue<CursorValue>();
        var rawCursor = cursorValue.NextCursor.OrElse(() =>
        {
            return cursorValue.Cursor.OrElse(() => throw new InvalidOperationException("Cursor is null, has a position been set of this cursor?"));
        });
        if (rawCursor is not IndexCursor indexCursor)
        {
            throw new NotSupportedException("can only seek on an index");
        }

        if (indexCursor.Index.Structure.Keys.Count != 1)
        {
            throw new NotImplementedException("handle multi-column indexes");
        }
        
        (state, var value) = state.PopValue<ObjectValue>();

        var newCursor = indexCursor.Search(new IndexKey(new [] { new ColumnValue(value.Value) }));

        state = state.PushValue(cursorValue.SetNextCursor(newCursor));

        return (state, new OperationResult.Next());
    }
}