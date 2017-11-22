namespace SimpleDatabase.Core.Execution.Values
{
    public class CursorValue : Value
    {
        public int RootPageNumber { get; }
        public bool Writable { get; }

        public Cursor Cursor { get; }

        public CursorValue(int rootPageNumber, bool writable)
        {
            RootPageNumber = rootPageNumber;
            Writable = writable;
            Cursor = null;
        }

        private CursorValue(CursorValue val, Cursor newCursor)
            : this(val.RootPageNumber, val.Writable)
        {
            Cursor = newCursor;
        }

        public CursorValue SetCursor(Cursor newCursor)
        {
            return new CursorValue(this, newCursor);
        }
    }
}
