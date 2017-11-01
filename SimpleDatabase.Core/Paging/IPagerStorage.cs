using System;

namespace SimpleDatabase.Core.Paging
{
    public interface IPagerStorage : IDisposable
    {
        int ByteLength { get; }

        Page Read(int index);
        void Write(Page page, int index, int size);
    }
}