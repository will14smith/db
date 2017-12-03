using SimpleDatabase.Execution;
using SimpleDatabase.Execution.Operations.Cursors;

namespace SimpleDatabase.Planning.Iterators
{
    public class DeleteIterator : IIterator
    {
        private readonly IIterator _input;
        
        public DeleteIterator(IIterator input)
        {
            _input = input;

            Output = new IteratorOutput.Void(GenerateInsert);
        }

        public IteratorOutput Output { get; }
        public void GenerateInit(ProgramLabel emptyTarget)
        {
            _input.GenerateInit(emptyTarget);
        }

        public void GenerateMoveNext(ProgramLabel loopStartTarget)
        {
            _input.GenerateMoveNext(loopStartTarget);
        }

        public void GenerateInsert(IOperationGenerator generator)
        {
            var inner = (IteratorOutput.Row) _input.Output;

            inner.Cursor.Load(generator);
            generator.Emit(new DeleteOperation());
            inner.Cursor.Store(generator);
        }
    }
}
