using System.Collections.Generic;
using SimpleDatabase.Execution;
using SimpleDatabase.Execution.Operations;

namespace SimpleDatabase.Planning.Iterators
{
    public class DeleteIterator : IIterator
    {
        private readonly IIterator _input;

        public DeleteIterator(IIterator input)
        {
            _input = input;
        }

        public IReadOnlyDictionary<SlotLabel, SlotDefinition> Slots => _input.Slots;
        public IReadOnlyDictionary<FunctionLabel, Function> Functions => _input.Functions;
        public IReadOnlyList<IteratorOutput> Outputs => new IteratorOutput[0];

        public IEnumerable<IOperation> Init(ProgramLabel emptyTarget)
        {
            return _input.Init(emptyTarget);
        }

        public IEnumerable<IOperation> MoveNext(ProgramLabel loopStartTarget)
        {
            return _input.MoveNext(loopStartTarget);
        }

        public IEnumerable<IOperation> Yield()
        {
            // TODO how to access the _input cursor?

            throw new System.NotImplementedException();
        }
    }
}
