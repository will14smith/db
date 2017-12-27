using System;
using SimpleDatabase.Schemas;

namespace SimpleDatabase.Execution.Transactions
{
    public interface ITransaction : IDisposable
    {
        TransactionId Id { get; }

        void Commit();
        void Rollback();
    }
}