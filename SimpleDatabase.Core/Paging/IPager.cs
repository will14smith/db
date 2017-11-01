using System;

namespace SimpleDatabase.Core.Paging
{
    public interface IPager : IDisposable
    {
        int RowCount { get; }

        Page Get(int index);
        void Flush(int index, int pageSize);
    }
}