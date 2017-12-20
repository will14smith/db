using System.Collections.Generic;

namespace SimpleDatabase.Schemas
{
    public class KeyStructure
    {
        public IReadOnlyList<(Column, KeyOrdering)> Keys { get; }
        public IReadOnlyList<Column> Data { get; }

        public KeyStructure(IReadOnlyList<(Column, KeyOrdering)> keys, IReadOnlyList<Column> data)
        {
            Keys = keys;
            Data = data;
        }
    }
}
