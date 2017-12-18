using System;

namespace SimpleDatabase.Storage.Paging
{
    public interface IPageStorage : IDisposable
    {
        PageSource Source { get; }
        int PageCount { get; }

        Page Read(PageId id);
        void Write(Page page);
    }
}