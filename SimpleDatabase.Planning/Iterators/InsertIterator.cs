using System.Collections.Generic;
using System.Linq;
using SimpleDatabase.Execution;
using SimpleDatabase.Execution.Operations;
using SimpleDatabase.Execution.Operations.Cursors;
using SimpleDatabase.Execution.Operations.Slots;
using SimpleDatabase.Storage;

namespace SimpleDatabase.Planning.Iterators
{
    public class InsertIterator : IIterator
    {
        private readonly IIterator _input;
        private readonly StoredTable _table;

        private readonly SlotLabel _cursor = SlotLabel.Create();

        public InsertIterator(IIterator input, StoredTable table)
        {
            _input = input;
            _table = table;
        }

        public IReadOnlyDictionary<SlotLabel, SlotDefinition> Slots
        {
            get
            {
                var d = _input.Slots.ToDictionary(x => x.Key, x => x.Value);
                d.Add(_cursor, new SlotDefinition());

                return d;
            }
        }

        public IReadOnlyDictionary<FunctionLabel, Function> Functions => _input.Functions;
        public IReadOnlyList<IteratorOutput> Outputs => new IteratorOutput[0];

        public IEnumerable<IOperation> Init(ProgramLabel emptyTarget)
        {
            foreach (var op in _input.Init(emptyTarget))
            {
                yield return op;
            }

            yield return new OpenWriteOperation(_table);
            yield return new StoreOperation(_cursor);
        }

        public IEnumerable<IOperation> MoveNext(ProgramLabel loopStartTarget)
        {
            return _input.MoveNext(loopStartTarget);
        }

        public IEnumerable<IOperation> Yield()
        {
            var hasYielded = false;

            foreach (var op in _input.Yield())
            {
                if (op is YieldOperation)
                {
                    yield return new LoadOperation(_cursor);
                    yield return new InsertOperation();
                    hasYielded = true;
                }
                else
                {
                    yield return op;
                }
            }

            if (!hasYielded)
            {
                yield return new LoadOperation(_cursor);
                yield return new InsertOperation();
            }
        }
    }
}
