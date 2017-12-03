using System.Collections.Generic;
using SimpleDatabase.Execution.Operations;

namespace SimpleDatabase.Planning
{
    public static class OperationGeneratorExtensions
    {
        public static void Emit(this IOperationGenerator generator, IEnumerable<IOperation> operations)
        {
            foreach (var operation in operations)
            {
                generator.Emit(operation);
            }
        }
    }
}