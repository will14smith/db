using System.Collections.Generic;
using SimpleDatabase.Planning.Items;

namespace SimpleDatabase.Planning.Iterators
{
    public abstract class IteratorOutput
    {
        public class Row : IteratorOutput
        {
            public Row(Item cursor, IReadOnlyList<Named> columns)
            {
                Cursor = cursor;
                Columns = columns;
            }

            public Item Cursor { get; }
            public IReadOnlyList<Named> Columns { get; }
        }

        public class Named : IteratorOutput
        {
            public IteratorOutputName Name { get; }
            public Item Value { get; }

            public Named(IteratorOutputName name, Item value)
            {
                Name = name;
                Value = value;
            }
        }

        public class Void : IteratorOutput
        {

        }
    }
}