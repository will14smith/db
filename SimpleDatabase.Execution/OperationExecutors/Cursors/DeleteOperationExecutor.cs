using System;
using SimpleDatabase.Execution.Operations.Cursors;
using SimpleDatabase.Execution.Values;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Execution.OperationExecutors.Cursors
{
    public class DeleteOperationExecutor : IOperationExecutor<DeleteOperation>
    {
        public (FunctionState, OperationResult) Execute(FunctionState state, DeleteOperation operation)
        {
            CursorValue cursorValue;
            (state, cursorValue) = state.PopValue<CursorValue>();

            var deleteTarget = cursorValue.Cursor.As<ICursor, IDeleteTarget>().OrElse(() => throw new InvalidOperationException("Cursor is null or not an IDeleteTarget"));

            var deleteResult = deleteTarget.Delete();
            switch (deleteResult)
            {
                case DeleteTargetResult.Success success:
                    state = state.PushValue(cursorValue.SetNextCursor(success.NextCursor));
                    return (state, new OperationResult.Next());

                default:
                    throw new NotImplementedException($"Unsupported type: {deleteResult.GetType().Name}");
            }
        }
    }
}
