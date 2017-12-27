using System;
using SimpleDatabase.Schemas;

namespace SimpleDatabase.Execution.Transactions
{
    public class Transaction : ITransaction
    {
        private readonly TransactionManager _manager;

        public TransactionId Id { get; }

        public Transaction(TransactionManager manager, TransactionId id)
        {
            _manager = manager;

            Id = id;
        }

        public void Commit()
        {
            if (_manager.Current.Id != Id)
            {
                throw new NotImplementedException();
            }
            _manager.CommitCurrent();
        }

        public void Rollback()
        {
            if (_manager.Current.Id != Id)
            {
                throw new NotImplementedException();
            }
            _manager.AbortCurrent();
        }

        public void Dispose()
        {
            if (!_manager.IsCommitted(Id))
            {
                Rollback();
            }
        }
    }

    public enum TransactionState
    {
        Unknown = 0,
        Active,
        Committed,
        Aborted
    }
}
