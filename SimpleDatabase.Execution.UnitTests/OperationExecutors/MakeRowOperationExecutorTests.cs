using System;
using SimpleDatabase.Execution.OperationExecutors;
using SimpleDatabase.Execution.Operations;
using SimpleDatabase.Execution.Values;
using Xunit;

namespace SimpleDatabase.Execution.UnitTests.OperationExecutors
{
    public class MakeRowOperationExecutorTests : OperationExecutorTestBase
    {
        [Fact]
        public void State_ShouldPopNItemsOffTheStackAndPushTheRow()
        {
            var value1 = RandomValue;
            var value2 = RandomValue;
            var row = new RowValue(new[] { value1, value2 });

            var initialState = BlankState.PushValue(value1).PushValue(value2);
            var expectedState = BlankState.PushValue(row);

            var (resultState, _) = new MakeRowOperationExecutor().Execute(initialState, new MakeRowOperation(2));

            AssertEqualState(expectedState, resultState);
        }

        [Fact]
        public void Result_ShouldBeNext()
        {
            var (_, result) = new MakeRowOperationExecutor().Execute(BlankState, new MakeRowOperation(0));

            AssertEqualResult(new OperationResult.Next(), result);
        }

        [Fact]
        public void WhenStackHasTooFewItems_ShouldThrow()
        {
            Assert.Throws<InvalidOperationException>(() => new MakeRowOperationExecutor().Execute(BlankState, new MakeRowOperation(1)));
        }
    }
}