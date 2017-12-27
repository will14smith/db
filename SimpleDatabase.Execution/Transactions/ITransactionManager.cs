using SimpleDatabase.Schemas;

namespace SimpleDatabase.Execution.Transactions
{
    public interface ITransactionManager
    {
        ITransaction Current { get; }
        ITransaction Begin();

        bool IsCommitted(TransactionId tx);
    }
}