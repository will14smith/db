using SimpleDatabase.Execution.Operations.Sorting;
using SimpleDatabase.Execution.Values;

namespace SimpleDatabase.Execution.OperationExecutors.Sorting
{
    public class SorterCursorOperationExecutor : IOperationExecutor<SorterCursor>
    {
        public (FunctionState, OperationResult) Execute(FunctionState state, SorterCursor operation)
        {
            SorterValue sorter;
            (state, sorter) = state.PopValue<SorterValue>();

            var cursor = sorter.NewCursor();
            var cursorValue = new CursorValue(false).SetNextCursor(cursor);
            state = state.PushValue(cursorValue);

            return (state, new OperationResult.Next());
        }
    }
}