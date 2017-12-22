namespace SimpleDatabase.Storage.Paging
{
    public abstract class PageSource
    {
        public class Heap : PageSource
        {
            public Heap(string tableName)
            {
                TableName = tableName;
            }

            public string TableName { get; }

            protected bool Equals(Heap other)
            {
                return string.Equals(TableName, other.TableName);
            }

            public override bool Equals(object obj)
            {
                if (obj is null) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is Heap other && Equals(other);
            }

            public override int GetHashCode()
            {
                return TableName.GetHashCode();
            }
        }
        public class Index : PageSource
        {
            public Index(string tableName, string indexName)
            {
                TableName = tableName;
                IndexName = indexName;
            }

            public string TableName { get; }
            public string IndexName { get; }

            protected bool Equals(Index other)
            {
                return string.Equals(TableName, other.TableName) 
                    && string.Equals(IndexName, other.IndexName);
            }

            public override bool Equals(object obj)
            {
                if (obj is null) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is Index other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (TableName.GetHashCode() * 397) ^ IndexName.GetHashCode();
                }
            }
        }
    }
}