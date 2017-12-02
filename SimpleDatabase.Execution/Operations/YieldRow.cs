namespace SimpleDatabase.Execution.Operations
{
    /// <summary>
    /// ..., R -> ...
    /// 
    /// Yields the item at the top of the stack to the execution environment
    /// Generally R is a Row
    /// </summary>
    public class YieldOperation : IOperation
    {
        public override string ToString()
        {
            return "YIELD";
        }
    }
}
