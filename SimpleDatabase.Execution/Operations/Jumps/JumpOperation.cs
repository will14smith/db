namespace SimpleDatabase.Execution.Operations.Jumps
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