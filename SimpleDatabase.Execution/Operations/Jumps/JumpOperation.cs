﻿namespace SimpleDatabase.Execution.Operations.Jumps
{
    public class JumpOperation : IOperation
    {
        public ProgramLabel Address { get; }

        public JumpOperation(ProgramLabel address)
        {
            Address = address;
        }

        public override string ToString()
        {
            return $"J {Address}";
        }
    }
}