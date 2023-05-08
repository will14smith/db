namespace SimpleDatabase.CLI.Commands;

public class CommitTransactionCommand : ICommand
{
    private readonly REPLState _state;
    private readonly IREPLOutput _output;

    public CommitTransactionCommand(REPLState state, IREPLOutput output)
    {
        _state = state;
        _output = output;
    }

    public CommandResponse Handle(string[] args)
    {
        if (_state.Transaction == null)
        {
            return new CommandResponse.Invalid("Cannot commit transaction, there isn't one started");
        }
        
        _output.WriteLine("Committing transaction");

        _state.Transaction.Commit();
        _state.Transaction = null;
        
        return new CommandResponse.Success();
    }
}