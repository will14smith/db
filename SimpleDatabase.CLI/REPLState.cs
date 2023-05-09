using SimpleDatabase.Execution.Transactions;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage;
using SimpleDatabase.Storage.Paging;

namespace SimpleDatabase.CLI;

public class REPLState
{
    public REPLState(Pager pager, DatabaseManager databaseManager, TransactionManager transactionManager)
    {
        Pager = pager;
        DatabaseManager = databaseManager;
        TransactionManager = transactionManager;
    }

    public Pager Pager { get; }
    public DatabaseManager DatabaseManager { get; }
    public TransactionManager TransactionManager { get; }

    public ITransaction? Transaction { get; set; }

    public Database Database => new(DatabaseManager.GetAllTables());
}