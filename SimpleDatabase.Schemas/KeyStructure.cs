using System.Collections.Generic;
using System.Linq;

namespace SimpleDatabase.Schemas
{
    public class KeyStructure
    {
        public int KeyColumns { get; }
        public int DataColumns { get; }

        public IReadOnlyList<KeyOrdering> Ordering { get; }

        public KeyStructure(int keyColumns, int dataColumns, IReadOnlyList<KeyOrdering> ordering)
        {
            KeyColumns = keyColumns;
            DataColumns = dataColumns;
            Ordering = ordering;
        }

        public override string ToString()
        {
            return $"{{{KeyColumns}+{DataColumns},{string.Join("", Ordering.Select(x => x == KeyOrdering.Ascending ? "+" : "-"))}}}";
        }
    }

    public enum KeyOrdering
    {
        Ascending,
        Descending
    }
}
