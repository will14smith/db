using System;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Storage.Heap
{
    public class HeapPage
    {
        private readonly Page _page;
        private readonly HeapPageLayout _layout = new HeapPageLayout();

        private HeapPage(Page page)
        {
            _page = page;
        }

        public static HeapPage Read(Page page)
        {
            if (page.Type != PageType.Heap)
            {
                throw new InvalidOperationException($"Tried to read a {PageType.Heap} page but found a {page.Type} page instead");
            }

            return new HeapPage(page);
        }

        public byte ItemCount
        {
            get => _page[_layout.ItemCountOffset].Read<byte>();
            set => _page[_layout.ItemCountOffset].Write(value);
        }

        private Span<byte> GetItemPointerOffset(int index)
        {
            var itemOffset = _layout.HeaderSize + _layout.ItemPointerSize * index;

            return _page[itemOffset].Slice(0, _layout.ItemPointerSize);
        }

        private (ushort, ushort) GetItemPointer(int index)
        {
            var pointerOffset = GetItemPointerOffset(index);
            var offset = pointerOffset.Read<ushort>();
            var length = pointerOffset.Slice(_layout.ItemPointerOffsetSize).Read<ushort>();

            return (offset, length);
        }

        public Span<byte> GetItem(int index)
        {
            var (offset, length) = GetItemPointer(index);

            return _page[offset].Slice(0, length);
        }

        public void AddItem(Span<byte> item)
        {
            var length = item.Length;

            // find last item offset
            int lastOffset;
            if (ItemCount == 0)
                lastOffset = PageLayout.PageSize - 1;
            else
            {
                (_, lastOffset) = GetItemPointer(ItemCount - 1);
            }

            var offset = lastOffset - length;
            var index = ItemCount;

            var pointerOffset = GetItemPointerOffset(index);
            pointerOffset.Write((ushort)offset);
            pointerOffset.Slice(_layout.ItemPointerOffsetSize).Write((ushort)length);

            var itemPointer = _page[offset].Slice(0, length);

            item.CopyTo(itemPointer);

            ItemCount++;
        }

        public void RemoveItem(int index)
        {
            var (_, length) = GetItemPointer(index);

            for (var i = index + 1; i < ItemCount - 1; i++)
            {
                var (ioffset, ilength) = GetItemPointer(i);

                if (ilength > length)
                {
                    throw new NotImplementedException("TODO handle overlapping copies");
                }

                // move the item forward by "length"
                _page[ioffset].Slice(0, ilength).CopyTo(_page[ioffset + length]);
            }

            for (var i = index + 1; i < ItemCount - 1; i++)
            {
                GetItemPointerOffset(i).CopyTo(GetItemPointerOffset(i - 1));
            }
        }
    }
}
