using SimpleDatabase.Execution.Operations.Jumps;
using SimpleDatabase.Planning.Iterators;

namespace SimpleDatabase.Planning.Compilation;

public abstract class IteratorCompilationUnit : ICompilationUnit
{
    private readonly IIterator _iterator;

    public IteratorCompilationUnit(IIterator iterator)
    {
        _iterator = iterator;
    }

    public void Compile(IOperationGenerator generator)
    {
        var start = generator.NewLabel("loop start");
        var done = generator.NewLabel("loop done");

        _iterator.GenerateInit();
        
        generator.MarkLabel(start);
        _iterator.GenerateMoveNext(start, done);
        
        CompileBody(generator, _iterator.Output);
        
        generator.Emit(new JumpOperation(start));
        generator.MarkLabel(done);
    }

    protected abstract void CompileBody(IOperationGenerator generator, IteratorOutput output);
}