using SimpleDatabase.Execution.Operations.Sorting;
using SimpleDatabase.Execution.Values;

namespace SimpleDatabase.Execution.OperationExecutors.Sorting
{
    public class SorterSortOperationExecutor : IOperationExecutor<SorterSort>
    {
        public (FunctionState, OperationResult) Execute(FunctionState state, SorterSort operation)
        {
            SorterValue sorter;
            (state, sorter) = state.PopValue<SorterValue>();

            sorter.Sort();

            return (state, new OperationResult.Next());
        }
    }
}