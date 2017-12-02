using SimpleDatabase.Execution.Trees;
using SimpleDatabase.Storage;

namespace SimpleDatabase.Execution.Values
{
    public class CursorValue : Value
    {
        public StoredTable Table { get; }
        public bool Writable { get; }

        public Cursor Cursor { get; }

        public CursorValue(StoredTable table, bool writable)
        {
            Table = table;
            Writable = writable;
            Cursor = null;
        }

        private CursorValue(CursorValue val, Cursor newCursor)
            : this(val.Table, val.Writable)
        {
            Cursor = newCursor;
        }

        public CursorValue SetCursor(Cursor newCursor)
        {
            return new CursorValue(this, newCursor);
        }
    }
}
