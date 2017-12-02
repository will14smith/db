using System;

namespace SimpleDatabase.Storage.Paging
{
    public class Page
    {
        public Page(int number, byte[] data)
        {
            Number = number;
            Data = data;
        }

        public int Number { get; }
        public byte[] Data { get; }

        public Span<byte> this[int index] => new Span<byte>(Data, index);
    }
}