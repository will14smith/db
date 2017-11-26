namespace SimpleDatabase.Execution.Operations.Jumps
{
    /// <summary>
    /// ..., x, y -> ...
    /// 
    /// Pops 2 values off stack then jumps to the address iff (x COMP y)
    /// </summary>
    public class ConditionalJumpOperation : IOperation
    {
        public Comparison Comparison { get; }
        public ProgramLabel Address { get; }

        public ConditionalJumpOperation(Comparison comparison, ProgramLabel address)
        {
            Address = address;
            Comparison = comparison;
        }
    }

    public enum Comparison
    {
        Equal,
        NotEqual
    }
}
