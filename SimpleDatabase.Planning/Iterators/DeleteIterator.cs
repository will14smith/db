using SimpleDatabase.Execution;
using SimpleDatabase.Execution.Operations.Cursors;

namespace SimpleDatabase.Planning.Iterators
{
    public class DeleteIterator : IIterator
    {
        private readonly IOperationGenerator _generator;
        private readonly IIterator _input;

        public DeleteIterator(IOperationGenerator generator, IIterator input)
        {
            _generator = generator;
            _input = input;

            Output = new IteratorOutput.Void();
        }

        public IteratorOutput Output { get; }

        public void GenerateInit()
        {
            _input.GenerateInit();
        }

        public void GenerateMoveNext(ProgramLabel loopStart, ProgramLabel loopEnd)
        {
            _input.GenerateMoveNext(loopStart, loopEnd);

            var inner = (IteratorOutput.Row)_input.Output;
            
            inner.Cursor.Load(_generator);
            _generator.Emit(new DeleteOperation());
            inner.Cursor.Store(_generator);
        }

        public void Reset()
        {
            throw new System.NotImplementedException();
        }
    }
}
