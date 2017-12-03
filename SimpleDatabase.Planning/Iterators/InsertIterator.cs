using SimpleDatabase.Execution;
using SimpleDatabase.Execution.Operations.Cursors;
using SimpleDatabase.Planning.Items;
using SimpleDatabase.Storage;

namespace SimpleDatabase.Planning.Iterators
{
    public class InsertIterator : IIterator
    {
        private readonly IOperationGenerator _generator;
        private readonly IIterator _input;
        private readonly StoredTable _table;

        private readonly SlotItem _cursor;

        public InsertIterator(IOperationGenerator generator, IIterator input, StoredTable table)
        {
            _generator = generator;
            _input = input;
            _table = table;

            _cursor = new SlotItem(_generator.NewSlot(new SlotDefinition()));

            Output = new IteratorOutput.Void(GenerateInsert);
        }
        
        public IteratorOutput Output { get; }
        public void GenerateInit(ProgramLabel emptyTarget)
        {
            _input.GenerateInit(emptyTarget);

            _generator.Emit(new OpenWriteOperation(_table));
            _cursor.Store(_generator);
        }

        public void GenerateMoveNext(ProgramLabel loopStartTarget)
        {
            _input.GenerateMoveNext(loopStartTarget);
        }

        public void GenerateInsert(IOperationGenerator generator)
        {
            _input.Output.Load(generator);
            _cursor.Load(generator);
            generator.Emit(new InsertOperation());
        }
    }
}
