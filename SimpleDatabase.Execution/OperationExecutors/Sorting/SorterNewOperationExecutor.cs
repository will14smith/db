using System;
using System.Collections.Generic;
using System.Text;
using SimpleDatabase.Execution.Operations.Sorting;
using SimpleDatabase.Execution.Values;

namespace SimpleDatabase.Execution.OperationExecutors.Sorting
{
    public class SorterNewOperationExecutor : IOperationExecutor<SorterNew>
    {
        public (FunctionState, OperationResult) Execute(FunctionState state, SorterNew operation)
        {
            var sorter = new SorterValue(operation.Key);

            state = state.PushValue(sorter);

            return (state, new OperationResult.Next());
        }
    }
}
