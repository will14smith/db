using SimpleDatabase.Core;

namespace SimpleDatabase.CLI.PrepareStatementResponses
{
    public class SuccessPrepareStatementResponses : IPrepareStatementResponse
    {
        public SuccessPrepareStatementResponses(IStatement statement)
        {
            Statement = statement;
        }

        public IStatement Statement { get; }
    }
}