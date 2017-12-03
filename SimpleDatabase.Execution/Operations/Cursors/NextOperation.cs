namespace SimpleDatabase.Execution.Operations.Cursors
{
    /// <summary>
    /// ..., T1 : ReadOnlyCursor -> ..., T2
    /// 
    /// Creates a cursor T2 pointing to the next entry after T1
    /// If T1 referred to the last entry it jumps to DoneAddress, otherwise it moves to the next instruction
    /// </summary>
    public class NextOperation : IOperation
    {
        public ProgramLabel DoneAddress { get; }

        public NextOperation(ProgramLabel doneAddress, bool upgrade)
        {
            DoneAddress = doneAddress;
        }

        public override string ToString()
        {
            return $"CUR.NEXT {DoneAddress}";
        }
    }
}
