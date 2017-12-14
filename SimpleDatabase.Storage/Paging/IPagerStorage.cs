using System;

namespace SimpleDatabase.Storage.Paging
{
    public interface IPagerStorage : IDisposable
    {
        int ByteLength { get; }

        Page Read(PageId id);
        void Write(Page page);
    }
}