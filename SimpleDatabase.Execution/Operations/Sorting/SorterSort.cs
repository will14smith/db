namespace SimpleDatabase.Execution.Operations.Sorting
{
    /// <summary>
    /// ..., S -> ...
    /// 
    /// Pops the sorter from the stack and sorts it.
    /// </summary>
    public class SorterSort : IOperation
    {
        public override string ToString()
        {
            return "SORTER.SORT";
        }
    }
}
