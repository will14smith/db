using System;
using System.Collections.Generic;
using SimpleDatabase.Execution.Operations;
using SimpleDatabase.Planning.Items;

namespace SimpleDatabase.Planning.Iterators
{
    public abstract class IteratorOutput
    {
        public abstract Item Load(IOperationGenerator generator);

        public class Row : IteratorOutput
        {
            public Row(Item cursor, IReadOnlyList<Named> columns)
            {
                Cursor = cursor;
                Columns = columns;
            }

            public Item Cursor { get; }
            public IReadOnlyList<Named> Columns { get; }

            public override Item Load(IOperationGenerator generator)
            {
                foreach (var col in Columns)
                {
                    col.Load(generator);
                }

                generator.Emit(new MakeRowOperation(Columns.Count));

                return new StackItem();
            }
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

            public override Item Load(IOperationGenerator generator)
            {
                return Value.Load(generator);
            }
        }

        public class Void : IteratorOutput
        {
            private readonly Action<IOperationGenerator> _load;

            public Void(Action<IOperationGenerator> load)
            {
                _load = load;
            }

            public override Item Load(IOperationGenerator generator)
            {
                _load(generator);
                return new VoidItem();
            }
        }
    }
}