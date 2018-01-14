using System;
using SimpleDatabase.Execution.OperationExecutors;
using SimpleDatabase.Execution.Operations;
using Xunit;

namespace SimpleDatabase.Execution.UnitTests.OperationExecutors
{
    public class YieldOperationExecutorTests : OperationExecutorTestBase
    {
        [Fact]
        public void State_ShouldPopSingleItem()
        {
            var (resultState, _) = new YieldOperationExecutor().Execute(BlankState.PushValue(RandomValue), new YieldOperation());

            AssertEqualState(BlankState, resultState);
        }

        [Fact]
        public void Result_ShouldYieldTheItemFromTheStack()
        {
            var value = RandomValue;

            var (_, result) = new YieldOperationExecutor().Execute(BlankState.PushValue(value), new YieldOperation());

            AssertEqualResult(new OperationResult.Yield(new OperationResult.Next(), value), result);
        }

        [Fact]
        public void WhenStackIsEmpty_ShouldThrow()
        {
            Assert.Throws<InvalidOperationException>(() => new YieldOperationExecutor().Execute(BlankState, new YieldOperation()));
        }
    }
}