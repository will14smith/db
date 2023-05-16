namespace SimpleDatabase.Execution.Operations.Cursors;

/// <summary>
/// ..., V0, IndexCursor -> IndexCursor
///
/// Moves the index cursor such that the next item will have a key[0] equal to V0
/// TODO support multiple values and partial left equality
/// </summary>
public class SeekEqualOperation : IOperation
{
    public override string ToString()
    {
        return "CUR.SEEK.EQ";
    }
}