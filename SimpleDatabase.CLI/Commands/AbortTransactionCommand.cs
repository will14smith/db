namespace SimpleDatabase.CLI.Commands;

public class AbortTransactionCommand : ICommand
{
    private readonly REPLState _state;
    private readonly IREPLOutput _output;

    public AbortTransactionCommand(REPLState state, IREPLOutput output)
    {
        _state = state;
        _output = output;
    }

    public CommandResponse Handle(string[] args)
    {
        if (_state.Transaction == null)
        {
            return new CommandResponse.Invalid("Cannot abort transaction, there isn't one started");
        }
                
        _output.WriteLine("Aborting transaction");

        _state.Transaction.Rollback();
        _state.Transaction = null;
        
        return new CommandResponse.Success();
    }
}