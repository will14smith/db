namespace SimpleDatabase.Execution.Operations.Cursors
{
    /// <summary>
    /// ..., T1 : ReadOnlyCursor -> ..., T2
    /// 
    /// Moves the cursor to a position such that the next element is the first item
    /// </summary>
    public class FirstOperation : IOperation
    {
        public override string ToString()
        {
            return "CUR.FIRST";
        }
    }
}
