using System;

namespace SimpleDatabase.Core.Paging
{
    public interface IPager : IDisposable
    {
        int PageCount { get; }
        int RowCount { get; }

        Page Get(int index);
        void Flush(int index);

        Page Allocate();
        void Free(int index);
    }
}