using SimpleDatabase.Execution.Operations;
using SimpleDatabase.Planning.Iterators;

namespace SimpleDatabase.Planning.Compilation;

public class YieldIteratorCompilationUnit : IteratorCompilationUnit
{
    public YieldIteratorCompilationUnit(IIterator iterator) : base(iterator) { }

    protected override void CompileBody(IOperationGenerator generator, IteratorOutput output)
    {
        if (output is IteratorOutput.Void)
        {
            return;
        }

        output.Load(generator);
        generator.Emit(new YieldOperation());
    }
}