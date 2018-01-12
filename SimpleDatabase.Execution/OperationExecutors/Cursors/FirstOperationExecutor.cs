using System;
using SimpleDatabase.Execution.Operations.Cursors;
using SimpleDatabase.Execution.Values;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Execution.OperationExecutors.Cursors
{
    public class FirstOperationExecutor : IOperationExecutor<FirstOperation>
    {
        public (FunctionState, OperationResult) Execute(FunctionState state, FirstOperation operation)
        {
            CursorValue cursorValue;
            (state, cursorValue) = state.PopValue<CursorValue>();

            var cursor = cursorValue.Cursor.OrElse(() =>
            {
                return cursorValue.NextCursor.OrElse(() => throw new InvalidOperationException("Something is right dodgy with this cursor..."));
            });

            var newCursor = cursor.First();
            var newCursorValue = cursorValue.SetNextCursor(newCursor);
            state = state.PushValue(newCursorValue);

            return (state, new OperationResult.Next());
        }
    }
}
