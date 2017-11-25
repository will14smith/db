namespace SimpleDatabase.Core.Execution.Operations
{
    public class JumpOperation : Operation
    {
        public int Address { get; }

        public JumpOperation(int address)
        {
            Address = address;
        }
    }
}