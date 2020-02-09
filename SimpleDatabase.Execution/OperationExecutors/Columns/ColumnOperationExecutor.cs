using System;
using SimpleDatabase.Execution.Operations.Columns;
using SimpleDatabase.Execution.Values;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Execution.OperationExecutors.Columns
{
    public class ColumnOperationExecutor : IOperationExecutor<ColumnOperation>
    {
        public (FunctionState, OperationResult) Execute(FunctionState state, ColumnOperation operation)
        {
            Value? value;
            (state, value) = state.PopValue();

            switch (value)
            {
                case CursorValue cursor: return Execute(state, operation, cursor);
                case RowValue row: return Execute(state, operation, row);

                default: throw new ArgumentOutOfRangeException(nameof(value), $"Unhandled type: {value?.GetType().FullName ?? "null"}");
            }
        }

        private static (FunctionState, OperationResult) Execute(FunctionState state, ColumnOperation operation, CursorValue cursorValue)
        {
            var cursor = cursorValue.Cursor.OrElse(() => throw new InvalidOperationException("Cursor is null, has a position been set of this cursor?"));

            var value = cursor.Column(operation.ColumnIndex);
            state.PushValue(new ObjectValue(value));

            return (state, new OperationResult.Next());
        }

        private static (FunctionState, OperationResult) Execute(FunctionState state, ColumnOperation operation, RowValue rowValue)
        {
            var value = rowValue.Values[operation.ColumnIndex];
            state.PushValue(new ObjectValue(value));

            return (state, new OperationResult.Next());
        }
    }
}
