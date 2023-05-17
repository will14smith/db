namespace SimpleDatabase.Planning.Compilation;

public interface ICompilationUnit
{
    void Compile(IOperationGenerator generator);
}