using SimpleDatabase.Schemas;

namespace SimpleDatabase.Execution.Operations.Cursors
{
    /// <summary>
    /// ... -> ..., ReadOnlyCursor
    ///    
    /// Aquires a read lock on the database
    /// Opens a read-only cursor on the specified index
    /// </summary>
    public class OpenReadIndexOperation : IOperation
    {
        public Table Table { get; }
        public Index Index { get; }

        public OpenReadIndexOperation(Table table, Index index)
        {
            Table = table;
            Index = index;
        }

        public override string ToString()
        {
            return $"CUR.OPEN.RI {Table.Name}.{Index.Name}";
        }
    }
}