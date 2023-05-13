using System;

namespace SimpleDatabase.Storage.Paging
{
    public abstract class PageSource : IEquatable<PageSource>
    {
        public abstract bool Equals(PageSource? other);

        public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is PageSource other && Equals(other);

        public abstract override int GetHashCode();

        public static bool operator ==(PageSource? left, PageSource? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PageSource? left, PageSource? right)
        {
            return !Equals(left, right);
        }

        public class Database : PageSource
        {
            public static readonly Database Instance = new();
            private Database() { }

            public override bool Equals(PageSource? other) => other is Database;
            public override int GetHashCode() => 7;
        }
        
        public class Table : PageSource
        {
            public Table(string tableName)
            {
                TableName = tableName;
            }

            public string TableName { get; }


            public override bool Equals(PageSource? other) => other is Table otherTable && string.Equals(TableName, otherTable.TableName);
            public override int GetHashCode() => unchecked(9 + TableName.GetHashCode());
        }

    }
}