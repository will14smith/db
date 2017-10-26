namespace SimpleDatabase.CLI.PrepareStatementResponses
{
    public class UnrecognisedPrepareStatementResponses : IPrepareStatementResponse
    {
        public UnrecognisedPrepareStatementResponses(string input)
        {
            Input = input;
        }

        public string Input { get; }
    }
}