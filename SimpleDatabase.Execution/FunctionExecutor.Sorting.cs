using SimpleDatabase.Execution.Operations.Sorting;
using SimpleDatabase.Execution.Values;

namespace SimpleDatabase.Execution
{
    public partial class FunctionExecutor
    {
        private (FunctionState, Result) Execute(FunctionState state, SorterNew op)
        {
            var sorter = new SorterValue(op.Key);

            state = state.PushValue(sorter);

            return (state, new Result.Next());
        }

        private (FunctionState, Result) Execute(FunctionState state, SorterSort op)
        {
            SorterValue sorter;
            (state, sorter) = state.PopValue<SorterValue>();

            sorter.Sort();

            return (state, new Result.Next());
        }

        private (FunctionState, Result) Execute(FunctionState state, SorterCursor op)
        {
            SorterValue sorter;
            (state, sorter) = state.PopValue<SorterValue>();

            var cursor = sorter.NewCursor();
            var cursorValue = new CursorValue(false).SetNextCursor(cursor);
            state = state.PushValue(cursorValue);

            return (state, new Result.Next());
        }
    }
}
