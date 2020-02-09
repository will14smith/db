using SimpleDatabase.Schemas;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Execution.Transactions
{
    public static class TransactionManagerExtensions
    {
        public static bool IsVisible(this ITransactionManager txm, TransactionId min, TransactionId? maxopt)
        {
            var tx = txm.Current.Id;

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