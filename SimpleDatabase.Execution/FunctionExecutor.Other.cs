using System;
using SimpleDatabase.Execution.Operations;
using SimpleDatabase.Execution.Values;

namespace SimpleDatabase.Execution
{
    public partial class FunctionExecutor
    {
        private static (FunctionState, Result) Execute(FunctionState state, MakeRowOperation makeRowOperation)
        {
            if (state.StackCount < makeRowOperation.ColumnCount)
            {
                throw new InvalidOperationException("Stack doesn't contain enough elements for this row");
            }

            var row = new Value[makeRowOperation.ColumnCount];

            for (var i = makeRowOperation.ColumnCount - 1; i >= 0; i--)
            {
                (state, row[i]) = state.PopValue();
            }

            state = state.PushValue(new RowValue(row));

            return (state, new Result.Next());
        }

        private static (FunctionState, Result) Execute(FunctionState state, YieldOperation yield)
        {
            Value value;
            (state, value) = state.PopValue();

            return (state, new Result.Yield(new Result.Next(), value));
        }
    }
}
