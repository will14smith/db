using System;
using SimpleDatabase.Execution.Operations.Columns;
using SimpleDatabase.Execution.Values;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Execution
{
    public partial class FunctionExecutor
    {
        private (FunctionState, Result) Execute(FunctionState state, KeyOperation _)
        {
            CursorValue cursorValue;
            (state, cursorValue) = state.PopValue<CursorValue>();

            var cursor = cursorValue.Cursor.OrElse(() => throw new InvalidOperationException("Cursor is null, has a position been set of this cursor?"));

            var key = cursor.Key();
            state.PushValue(new ObjectValue(key));

            return (state, new Result.Next());
        }

        private (FunctionState, Result) Execute(FunctionState state, ColumnOperation columnOperation)
        {
            Value value;
            (state, value) = state.PopValue();

            switch (value)
            {
                case CursorValue cursor: return Execute(state, columnOperation, cursor);
                case RowValue row: return Execute(state, columnOperation, row);

                default: throw new ArgumentOutOfRangeException(nameof(value), $"Unhandled type: {value.GetType().FullName}");
            }
        }

        private (FunctionState, Result) Execute(FunctionState state, ColumnOperation columnOperation, CursorValue cursorValue)
        {
            var cursor = cursorValue.Cursor.OrElse(() => throw new InvalidOperationException("Cursor is null, has a position been set of this cursor?"));

            var value = cursor.Column(columnOperation.ColumnIndex);
            state.PushValue(new ObjectValue(value));

            return (state, new Result.Next());
        }

        private (FunctionState, Result) Execute(FunctionState state, ColumnOperation columnOperation, RowValue rowValue)
        {
            var value = rowValue.Values[columnOperation.ColumnIndex];
            state.PushValue(new ObjectValue(value));

            return (state, new Result.Next());
        }
    }
}
