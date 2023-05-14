namespace SimpleDatabase.Execution.Operations.Cursors;

/// <summary>
/// ..., RowId, T1 : ReadOnlyCursor -> ..., T2
/// 
/// Moves the cursor to a position such that the next element is the heap item with the row id
/// </summary>
public class SeekRowIdOperation : IOperation
{
    public override string ToString()
    {
        return "SEEK.ROWID";
    }
}
