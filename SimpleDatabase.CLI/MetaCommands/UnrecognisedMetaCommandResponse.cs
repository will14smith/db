namespace SimpleDatabase.CLI.MetaCommands
{
    public class UnrecognisedMetaCommandResponse : IMetaCommandResponse
    {
        public string Input { get; }

        public UnrecognisedMetaCommandResponse(string input)
        {
            Input = input;
        }
    }
}