using SimpleDatabase.Execution.Operations.Cursors;
using SimpleDatabase.Execution.Transactions;
using SimpleDatabase.Storage;

namespace SimpleDatabase.Execution.OperationExecutors.Cursors;

public class SeekRowIdOperationExecutor : IOperationExecutor<SeekRowIdOperation>
{
    private readonly DatabaseManager _dbm;
    private readonly ITransactionManager _txm;

    public SeekRowIdOperationExecutor(DatabaseManager dbm, ITransactionManager txm)
    {
        _dbm = dbm;
        _txm = txm;
    }

    public (FunctionState, OperationResult) Execute(FunctionState state, SeekRowIdOperation operation)
    {
        (state, var cursor) = state.PopValue(); 
        (state, var rowId) = state.PopValue(); 
        
        throw new System.NotImplementedException();
    }
}