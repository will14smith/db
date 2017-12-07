using System;
using SimpleDatabase.Schemas;

namespace SimpleDatabase.Execution
{
    public static class RowExtensions
    {
        public static int GetKey(this Row row)
        {
            var value0 = row.Values[0].Value;

            if (value0 is int i) return i;

            throw new NotImplementedException();
        }
    }
}