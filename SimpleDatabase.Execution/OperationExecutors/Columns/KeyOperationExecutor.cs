using System;
using SimpleDatabase.Execution.Operations.Columns;
using SimpleDatabase.Execution.Values;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Execution.OperationExecutors.Columns
{
    public class KeyOperationExecutor : IOperationExecutor<KeyOperation>
    {
        public (FunctionState, OperationResult) Execute(FunctionState state, KeyOperation operation)
        {
            CursorValue cursorValue;
            (state, cursorValue) = state.PopValue<CursorValue>();

            var cursor = cursorValue.Cursor.OrElse(() => throw new InvalidOperationException("Cursor is null, has a position been set of this cursor?"));

            var key = cursor.Key();
            state.PushValue(new ObjectValue(key));

            return (state, new OperationResult.Next());
        }
    }
}
