namespace SimpleDatabase.Execution.Operations
{
    /// <summary>
    /// [] -> n/a
    /// 
    /// Finishes the query, clears anything left open up (transactions, locks, etc...)
    /// </summary>
    public class FinishOperation : IOperation
    {
        public override string ToString()
        {
            return "FINISH";
        }
    }
}
