using SimpleDatabase.Schemas;

namespace SimpleDatabase.Execution.Operations.Cursors
{
    /// <summary>
    /// ... -> ..., WritableCursor
    ///    
    /// Aquires a write lock on the database
    /// Opens a read/write cursor on the b-tree with a root at the specified page number
    /// </summary>
    public class OpenWriteOperation : IOperation
    {
        public Table Table { get; }

        public OpenWriteOperation(Table table)
        {
            Table = table;
        }

        public override string ToString()
        {
            return $"CUR.OPEN.W {Table.Name}";
        }
    }
}
