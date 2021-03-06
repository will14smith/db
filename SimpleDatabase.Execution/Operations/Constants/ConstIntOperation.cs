﻿namespace SimpleDatabase.Execution.Operations.Constants
{
    /// <summary>
    /// ... -> ..., x
    /// 
    /// Pushes a constant int on to the stack
    /// </summary>
    public class ConstIntOperation : IOperation
    {
        public int Value { get; }

        public ConstIntOperation(int value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return $"CONST.I {Value}";
        }
    }
}
