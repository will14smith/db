using SimpleDatabase.Utils;

namespace SimpleDatabase.Execution.Values
{
    public class CursorValue : Value
    {
        public bool Writable { get; }

        public Option<ICursor> Cursor { get; }
        public Option<ICursor> NextCursor { get; }

        public CursorValue(bool writable)
        {
            Writable = writable;
            Cursor = Option.None<ICursor>();
        }

        private CursorValue(CursorValue val, ICursor cursor)
            : this(val.Writable)
        {
            Cursor = Option.Some(cursor);
        }
        private CursorValue(CursorValue val, Option<ICursor> cursor, ICursor nextCursor)
            : this(val.Writable)
        {
            Cursor = cursor;
            NextCursor = Option.Some(nextCursor);
        }

        public CursorValue SetCursor(ICursor newCursor)
        {
            return new CursorValue(this, newCursor);
        }

        public CursorValue SetNextCursor(ICursor newCursor)
        {
            return new CursorValue(this, Cursor, newCursor);
        }
    }
}