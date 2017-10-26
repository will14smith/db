namespace SimpleDatabase.CLI.MetaCommands
{
    public class ExitMetaCommandResponse : IMetaCommandResponse
    {
        public ExitCode Code { get; }

        public ExitMetaCommandResponse(ExitCode code)
        {
            Code = code;
        }
    }
}