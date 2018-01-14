using System.Collections.Generic;
using Xunit;

namespace SimpleDatabase.Execution.UnitTests.OperationExecutors
{
    public abstract class OperationExecutorTestBase
    {
        public FunctionState BlankState => new FunctionState(new Dictionary<SlotLabel, SlotDefinition>());

        public void AssertEqualState(FunctionState expected, FunctionState actual)
        {
            Assert.Equal(expected, actual, FunctionState.EqualityComparer);
        }

        public void AssertEqualResult(OperationResult expected, OperationResult actual)
        {
            switch (expected)
            {
                case OperationResult.Next _:
                    Assert.IsType<OperationResult.Next>(actual);
                    break;
                case OperationResult.Jump expectedJump:
                    var actualJump = Assert.IsType<OperationResult.Jump>(actual);
                    Assert.Equal(expectedJump.Address, actualJump.Address);
                    break;
                case OperationResult.Finished _:
                    Assert.IsType<OperationResult.Finished>(actual);
                    break;
                case OperationResult.Yield expectedYield:
                    var actualYield = Assert.IsType<OperationResult.Yield>(actual);

                    AssertEqualResult(expectedYield.Inner, actualYield.Inner);
                    Assert.Equal(expectedYield.Value, actualYield.Value);
                    break;

                default:
                    Assert.False(true);
                    break;
            }
        }
    }
}