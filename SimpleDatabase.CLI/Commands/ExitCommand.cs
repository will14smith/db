namespace SimpleDatabase.CLI.Commands;

internal class ExitCommand : ICommand
{
    public CommandResponse Handle(string[] args) => new CommandResponse.Exit(ExitCode.Success);
}