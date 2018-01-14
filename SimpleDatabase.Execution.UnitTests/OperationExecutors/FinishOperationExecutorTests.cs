using SimpleDatabase.Execution.OperationExecutors;
using SimpleDatabase.Execution.Operations;
using Xunit;

namespace SimpleDatabase.Execution.UnitTests.OperationExecutors
{
    public class FinishOperationExecutorTests : OperationExecutorTestBase
    {
        [Fact]
        public void State_ShouldNotChange()
        {
            var (resultState, _) = new FinishOperationExecutor().Execute(BlankState, new FinishOperation());

            AssertEqualState(BlankState, resultState);
        }

        [Fact]
        public void Result_ShouldBeFinished()
        {
            var (_, result) = new FinishOperationExecutor().Execute(BlankState, new FinishOperation());

            AssertEqualResult(new OperationResult.Finished(), result);
        }
    }
}
