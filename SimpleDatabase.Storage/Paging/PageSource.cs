namespace SimpleDatabase.Storage.Paging
{
    public abstract class PageSource
    {
        public class Database : PageSource
        {
            public static readonly Database Instance = new();
            private Database() { }
            
            public override bool Equals(object? obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                
                return obj is Database;
            }

            public override int GetHashCode()
            {
                return 7;
            }
        }
        
        public class Table : PageSource
        {
            public Table(string tableName)
            {
                TableName = tableName;
            }

            public string TableName { get; }

            protected bool Equals(Table other)
            {
                return string.Equals(TableName, other.TableName);
            }

            public override bool Equals(object? obj)
            {
                if (obj is null) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is Table other && Equals(other);
            }

            public override int GetHashCode()
            {
                return unchecked(9 + TableName.GetHashCode());
            }
        }

    }
}