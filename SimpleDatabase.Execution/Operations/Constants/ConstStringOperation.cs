namespace SimpleDatabase.Execution.Operations.Constants
{
    /// <summary>
    /// ... -> ..., x
    /// 
    /// Pushes a constant string on to the stack
    /// </summary>
    public class ConstStringOperation : IOperation
    {
        public string Value { get; }

        public ConstStringOperation(string value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return $"CONST.S {Value}";
        }
    }
}
