using System;
using SimpleDatabase.Execution.Operations.Cursors;
using SimpleDatabase.Execution.Values;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Execution.OperationExecutors.Cursors
{
    public class NextOperationExecutor : IOperationExecutor<NextOperation>
    {
        public (FunctionState, OperationResult) Execute(FunctionState state, NextOperation operation)
        {
            CursorValue cursorValue;
            (state, cursorValue) = state.PopValue<CursorValue>();

            var newCursor = cursorValue.NextCursor.OrElse(() =>
            {
                var cursor = cursorValue.Cursor.OrElse(() => throw new InvalidOperationException("Cursor is null, has a position been set of this cursor?"));

                return cursor.Next();
            });

            var newCursorValue = cursorValue.SetCursor(newCursor);
            state = state.PushValue(newCursorValue);

            if (!newCursor.EndOfTable)
            {
                return (state, new OperationResult.Next());
            }

            return (state, new OperationResult.Jump(operation.DoneAddress));
        }
    }
}
