using SimpleDatabase.Schemas;

namespace SimpleDatabase.Execution.Operations.Sorting
{
    /// <summary>
    /// ... -> ..., S
    /// 
    /// Creates a new sorter with a specified key structure and pushes it to the stack.
    /// </summary>
    public class SorterNew : IOperation
    {
        public KeyStructure Key { get; }

        public SorterNew(KeyStructure key)
        {
            Key = key;
        }

        public override string ToString()
        {
            return $"SORTER.NEW {Key}";
        }
    }
}
