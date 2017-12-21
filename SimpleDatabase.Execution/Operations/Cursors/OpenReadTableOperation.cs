using SimpleDatabase.Schemas;

namespace SimpleDatabase.Execution.Operations.Cursors
{
    /// <summary>
    /// ... -> ..., ReadOnlyCursor
    ///    
    /// Aquires a read lock on the database
    /// Opens a read-only cursor on the specified table
    /// </summary>
    public class OpenReadTableOperation : IOperation
    {
        public Table Table { get; }

        public OpenReadTableOperation(Table table)
        {
            Table = table;
        }

        public override string ToString()
        {
            return $"CUR.OPEN.RT {Table.Name}";
        }
    }
}
