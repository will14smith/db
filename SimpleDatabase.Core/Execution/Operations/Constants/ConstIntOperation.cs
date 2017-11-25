namespace SimpleDatabase.Core.Execution.Operations
{
    /// <summary>
    /// ... -> ..., x
    /// 
    /// Pushes a constant int on to the stack
    /// </summary>
    public class ConstIntOperation : Operation
    {
        public int Value { get; }

        public ConstIntOperation(int value)
        {
            Value = value;
        }
    }
}
