namespace SimpleDatabase.CLI.Commands;

public class BeginTransactionCommand : ICommand
{
    private readonly REPLState _state;
    private readonly IREPLOutput _output;

    public BeginTransactionCommand(REPLState state, IREPLOutput output)
    {
        _state = state;
        _output = output;
    }

    public CommandResponse Handle(string[] args)
    {
        _output.WriteLine("Beginning transaction");

        _state.Transaction = _state.TransactionManager.Begin();
        
        return new CommandResponse.Success();
    }
}