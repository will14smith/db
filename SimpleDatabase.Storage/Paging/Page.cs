using System;

namespace SimpleDatabase.Storage.Paging
{
    public class Page
    {
        public Page(PageId id, byte[] data)
        {
            Id = id;
            Data = data;
        }

        public PageId Id { get; }
        public byte[] Data { get; }
        
        public PageType Type
        {
            get => (PageType) Data[PageLayout.PageTypeOffset];
            set => Data[PageLayout.PageTypeOffset] = (byte) value;
        }

        public Span<byte> this[int index] => new Span<byte>(Data, index);
    }
}