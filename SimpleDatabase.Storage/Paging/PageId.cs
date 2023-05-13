using System;

namespace SimpleDatabase.Storage.Paging
{
    public class PageId : IEquatable<PageId>
    {
        public PageId(PageSource source, int index)
        {
            Source = source;
            Index = index;
        }

        public PageSource Source { get; }
        public int Index { get; }

        public bool Equals(PageId other)
        {
            return Source.Equals(other.Source) && Index == other.Index;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is PageId other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Source.GetHashCode() * 397) ^ Index;
            }
        }

        public static bool operator ==(PageId? left, PageId? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PageId? left, PageId? right)
        {
            return !Equals(left, right);
        }
    }
}