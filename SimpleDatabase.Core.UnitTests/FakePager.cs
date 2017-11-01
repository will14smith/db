using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleDatabase.Core.UnitTests
{
    class FakePager : IPager
    {
        public int RowCount { get; }
        public Page Get(int index)
        {
            throw new NotImplementedException();
        }

        public void Flush(int index, int pageSize)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
