namespace SimpleDatabase.Execution.Operations.Functions
{

    /// <summary>
    /// ..., a1, a2, ..., aN -> ..., R
    /// 
    /// Pops N items from the stack.
    /// Call F with the N values as arguments.
    /// Pushes the return value onto the stack.
    /// </summary>
    public class CallOperation : IOperation
    {
        public FunctionLabel Function { get; }
        public int ArgumentCount { get; }

        public CallOperation(FunctionLabel function, int argumentCount)
        {
            Function = function;
            ArgumentCount = argumentCount;
        }

        public override string ToString()
        {
            return $"CALL {Function} {ArgumentCount}";
        }
    }
}
