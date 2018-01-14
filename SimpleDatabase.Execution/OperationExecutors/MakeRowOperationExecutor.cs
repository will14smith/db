using System;
using SimpleDatabase.Execution.Operations;
using SimpleDatabase.Execution.Values;

namespace SimpleDatabase.Execution.OperationExecutors
{
    public class MakeRowOperationExecutor : IOperationExecutor<MakeRowOperation>
    {
        public (FunctionState, OperationResult) Execute(FunctionState state, MakeRowOperation operation)
        {
            if (state.StackCount < operation.ColumnCount)
            {
                throw new InvalidOperationException("Stack doesn't contain enough elements for this row");
            }

            var row = new Value[operation.ColumnCount];

            for (var i = operation.ColumnCount - 1; i >= 0; i--)
            {
                (state, row[i]) = state.PopValue();
            }

            state = state.PushValue(new RowValue(row));

            return (state, new OperationResult.Next());
        }
    }
}
