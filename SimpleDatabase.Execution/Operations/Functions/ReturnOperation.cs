namespace SimpleDatabase.Execution.Operations.Functions
{
    /// <summary>
    /// ..., R -> ...
    /// 
    /// Pops R from the stack and returns to the caller.
    /// If called from within a coroutine then the handle will be updated.
    /// </summary>
    public class ReturnOperation : IOperation
    {
        public override string ToString()
        {
            return "RET";
        }
    }
}