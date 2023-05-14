using SimpleDatabase.Execution;
using SimpleDatabase.Execution.Operations;

namespace SimpleDatabase.Planning
{
    public interface IOperationGenerator
    {
        FunctionLabel Label { get; }

        IOperationGenerator NewFunction();
        SlotLabel NewSlot(SlotDefinition definition);

        ProgramLabel NewLabel(string name);
        void MarkLabel(ProgramLabel label);
        
        void Emit(IOperation operation);
        void EmitNotImplemented(string message);
    }
}
