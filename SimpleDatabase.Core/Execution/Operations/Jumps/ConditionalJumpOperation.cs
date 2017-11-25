namespace SimpleDatabase.Core.Execution.Operations
{
    /// <summary>
    /// ..., x, y -> ...
    /// 
    /// Pops 2 values off stack then jumps to the address iff (x COMP y)
    /// </summary>
    public class ConditionalJumpOperation : Operation
    {
        public Comparison Comparison { get; }
        public int Address { get; }

        public ConditionalJumpOperation(Comparison comparison, int address)
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
