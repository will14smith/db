namespace SimpleDatabase.Execution.Operations
{
    /// <summary>
    /// ..., X -> ..., X, X
    /// 
    /// Duplicates the value at the top of the stack
    /// </summary>
    public class DupOperation : IOperation
    {
        public override string ToString()
        {
            return "DUP";
        }
    }
}
