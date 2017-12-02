namespace SimpleDatabase.Execution.Operations.Functions
{
    /// <summary>
    /// ..., a1, a2, ..., aN -> ..., handle
    /// 
    /// Pops N items from the stack.
    /// Calls C with the N values as arguments.
    /// Pushes the coroutine handle to the stack.
    /// </summary>
    public class SetupCoroutineOperation : IOperation
    {
        public FunctionLabel Function { get; }
        public int ArgumentCount { get; }

        public SetupCoroutineOperation(FunctionLabel function, int argumentCount)
        {
            Function = function;
            ArgumentCount = argumentCount;
        }

        public override string ToString()
        {
            return $"CO.SETUP {Function} {ArgumentCount}";
        }
    }
}