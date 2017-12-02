using System;

namespace SimpleDatabase.Storage.Paging
{
    public interface IPagerStorage : IDisposable
    {
        int ByteLength { get; }

        Page Read(int index);
        void Write(Page page, int index);
    }
}