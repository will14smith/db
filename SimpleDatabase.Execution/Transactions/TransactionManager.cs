using System;
using System.Collections.Concurrent;
using System.Threading;
using SimpleDatabase.Schemas;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Execution.Transactions
{
    public class TransactionManager : ITransactionManager
    {
        // TODO read latest xid
        private long _id = 1;

        private readonly ConcurrentDictionary<TransactionId, (TransactionState, TransactionId)> _states;
        
        private readonly ThreadLocal<ITransaction?> _current;

        public TransactionManager()
        {
            _states = new ConcurrentDictionary<TransactionId, (TransactionState, TransactionId)>();
            _current = new ThreadLocal<ITransaction?>();
        }

        public ITransaction? Current => _current.Value;

        public ITransaction Begin()
        {
            var id = new TransactionId((ulong)Interlocked.Increment(ref _id));

            if (!_states.TryAdd(id, (TransactionState.Active, id)))
            {
                throw new InvalidOperationException();
            }

            var tx = new Transaction(this, id);

            _current.Value = tx;

            return tx;
        }

        public bool IsCommitted(TransactionId tx)
        {
            var stateOpt = _states.TryGet(tx);
            if (!stateOpt.HasValue) return false;

            var (state, statetx) = stateOpt.Value;

            if (state != TransactionState.Committed) return false;
            // check the commit happened before this tx began
            return Current == null || statetx <= Current.Id;
        }

        internal void CommitCurrent()
        {
            if (Current is null) throw new InvalidOperationException("Cannot commit transaction since there isn't one started"); 
            
            var currentId = new TransactionId((ulong)Interlocked.Read(ref _id));

            _states[Current.Id] = (TransactionState.Committed, currentId);
            _current.Value = null;
        }
        internal void AbortCurrent()
        {
            if (Current is null) throw new InvalidOperationException("Cannot abort transaction since there isn't one started"); 

            var currentId = new TransactionId((ulong)Interlocked.Read(ref _id));

            _states[Current.Id] = (TransactionState.Aborted, currentId);
            _current.Value = null;
        }
    }
}
