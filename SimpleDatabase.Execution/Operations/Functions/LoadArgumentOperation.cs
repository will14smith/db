namespace SimpleDatabase.Execution.Operations.Functions
{
    /// <summary>
    /// ... -> ..., Ai
    /// 
    /// Pushes the ith argument onto the stack.
    /// </summary>
    public class LoadArgumentOperation : IOperation
    {
        public int Index { get; }

        public LoadArgumentOperation(int index)
        {
            Index = index;
        }

        public override string ToString()
        {
            return $"LOAD.A {Index}";
        }
    }
}