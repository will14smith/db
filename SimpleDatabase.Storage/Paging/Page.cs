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
        
        public PageType Type
        {
            get => (PageType) Data[PageLayout.PageTypeOffset];
            set => Data[PageLayout.PageTypeOffset] = (byte) value;
        }

        public Span<byte> this[int index] => new Span<byte>(Data, index);
    }
}