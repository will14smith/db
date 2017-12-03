using SimpleDatabase.Execution.Trees;
using SimpleDatabase.Storage;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Execution.Values
{
    public class CursorValue : Value
    {
        public StoredTable Table { get; }
        public bool Writable { get; }

        public Option<Cursor> Cursor { get; }
        public Option<Cursor> NextCursor { get; }

        public CursorValue(StoredTable table, bool writable)
        {
            Table = table;
            Writable = writable;
            Cursor = Option.None<Cursor>();
        }

        private CursorValue(CursorValue val, Cursor cursor)
            : this(val.Table, val.Writable)
        {
            Cursor = Option.Some(cursor);
        }
        private CursorValue(CursorValue val, Option<Cursor> cursor, Cursor nextCursor)
            : this(val.Table, val.Writable)
        {
            Cursor = cursor;
            NextCursor = Option.Some(nextCursor);
        }

        public CursorValue SetCursor(Cursor newCursor)
        {
            return new CursorValue(this, newCursor);
        }

        public CursorValue SetNextCursor(Cursor newCursor)
        {
            return new CursorValue(this, Cursor, newCursor);
        }
    }
}
