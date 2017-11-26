namespace SimpleDatabase.Execution.Operations.Cursors
{
    /// <summary>
    /// ... -> ..., ReadOnlyCursor
    ///    
    /// Aquires a read lock on the database
    /// Opens a read-only cursor on the b-tree with a root at the specified page number
    /// </summary>
    public class OpenReadOperation : Operation
    {
        public int RootPageNumber { get; }

        public OpenReadOperation(int rootPageNumber)
        {
            RootPageNumber = rootPageNumber;
        }
    }
}
