using SimpleDatabase.Execution.Transactions;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage;
using SimpleDatabase.Storage.Paging;

namespace SimpleDatabase.CLI;

public class REPLState
{
    public REPLState(Pager pager, Table table, Database database, TransactionManager transactionManager)
    {
        Pager = pager;
        Table = table;
        Database = database;
        TransactionManager = transactionManager;
    }

    public Pager Pager { get; }
    public Table Table { get; }
    public Database Database { get; }
    public TransactionManager TransactionManager { get; }

    public ITransaction? Transaction { get; set; }
}