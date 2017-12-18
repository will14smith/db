using System;

namespace SimpleDatabase.Storage.Paging
{
    public interface IPager : IDisposable
    {
        Page Get(PageId id);
        void Flush(PageId id);

        Page Allocate(PageSource source);
        void Free(PageId id);
    }
}