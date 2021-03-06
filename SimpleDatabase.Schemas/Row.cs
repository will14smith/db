﻿using System.Collections.Generic;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Schemas
{
    public class Row
    {
        public TransactionId MinXid { get; }
        public TransactionId? MaxXid { get; }

        public IReadOnlyList<ColumnValue> Values { get; }

        public Row(IReadOnlyList<ColumnValue> values, TransactionId minXid, TransactionId? maxXid)
        {
            Values = values;
            MinXid = minXid;
            MaxXid = maxXid;
        }
    }
}
