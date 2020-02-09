using SimpleDatabase.Schemas;

namespace SimpleDatabase.Execution.Transactions
{
    public static class TransactionManagerExtensions
    {
        public static bool IsVisible(this ITransactionManager txm, TransactionId min, TransactionId? maxopt)
        {
            // TODO is this the correct behaviour?
            var tx = txm.Current?.Id ?? new TransactionId(ulong.MaxValue);

            if (maxopt.HasValue)
            {
                var max = maxopt.Value;

                if (max < tx && txm.IsCommitted(max))
                {
                    // deleted & committed
                    return false;
                }
                if (max == tx)
                {
                    // deleted by this tx
                    return false;
                }
            }

            if (min < tx && txm.IsCommitted(min))
            {
                // inserted & committed
                return true;
            }
            if (min == tx)
            {
                // inserted by this tx
                return true;
            }
            
            return false;
        }
    }
}