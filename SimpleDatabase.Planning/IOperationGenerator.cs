using SimpleDatabase.Execution;
using SimpleDatabase.Execution.Operations;

namespace SimpleDatabase.Planning
{
    public interface IOperationGenerator
    {
        FunctionLabel Label { get; }

        IOperationGenerator NewFunction();
        SlotLabel NewSlot(SlotDefinition definition);

        ProgramLabel NewLabel();
        void MarkLabel(ProgramLabel label);
        
        void Emit(IOperation operation);
    }
}
