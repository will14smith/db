using SimpleDatabase.Execution;
using SimpleDatabase.Execution.Operations.Cursors;
using SimpleDatabase.Planning.Items;
using SimpleDatabase.Schemas;

namespace SimpleDatabase.Planning.Iterators
{
    public class InsertIterator : IIterator
    {
        private readonly IOperationGenerator _generator;
        private readonly IIterator _input;
        private readonly Table _table;

        private readonly SlotItem _cursor;

        public InsertIterator(IOperationGenerator generator, IIterator input, Table table)
        {
            _generator = generator;
            _input = input;
            _table = table;

            _cursor = new SlotItem(_generator.NewSlot(new SlotDefinition("insert cursor")));

            Output = new IteratorOutput.Void();
        }

        public IteratorOutput Output { get; }

        public void GenerateInit()
        {
            _input.GenerateInit();

            _generator.Emit(new OpenWriteOperation(_table));
            _cursor.Store(_generator);
        }

        public void GenerateMoveNext(ProgramLabel loopStart, ProgramLabel loopEnd)
        {
            _input.GenerateMoveNext(loopStart, loopEnd);

            _input.Output.Load(_generator);
            _cursor.Load(_generator);
            _generator.Emit(new InsertOperation());
        }
    }
}
