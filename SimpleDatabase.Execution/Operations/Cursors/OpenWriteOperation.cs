using System;
using System.Collections.Generic;
using System.Text;
using SimpleDatabase.Storage;

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
        public StoredTable Table { get; }

        public OpenWriteOperation(StoredTable table)
        {
            Table = table;
        }

        public override string ToString()
        {
            return $"CUR.OPEN.W {Table.Table.Name}";
        }
    }
}
