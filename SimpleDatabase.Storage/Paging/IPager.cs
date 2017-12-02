using System;

namespace SimpleDatabase.Storage.Paging
{
    public interface IPager : IDisposable
    {
        int PageCount { get; }

        Page Get(int index);
        void Flush(int index);

        Page Allocate();
        void Free(int index);
    }
}