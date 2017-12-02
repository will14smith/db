namespace SimpleDatabase.Execution.Operations.Functions
{
    /// <summary>
    /// ..., handle -> ..., R
    /// 
    /// Call a coroutine with a given handle and pushed the result onto the stack
    /// If the coroutine has completed then jumps to Done
    /// </summary>
    public class CallCoroutineOperation : IOperation
    {
        public ProgramLabel Done { get; }

        public CallCoroutineOperation(ProgramLabel done)
        {
            Done = done;
        }

        public override string ToString()
        {
            return $"CO.CALL {Done}";
        }
    }
}