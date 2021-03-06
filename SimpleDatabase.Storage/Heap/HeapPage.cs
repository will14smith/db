﻿using System;
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
        public static HeapPage New(Page page)
        {
            return new HeapPage(page)
            {
                Type = PageType.Heap,
                ItemCount = 0,
                NextPageIndex = 0,
            };
        }

        public PageId PageId => _page.Id;
        public PageType Type
        {
            get => _page.Type;
            private set => _page.Type = value;
        }

        public byte ItemCount
        {
            get => _page[_layout.ItemCountOffset].Read<byte>();
            private set => _page[_layout.ItemCountOffset].Write(value);
        }
        public int NextPageIndex
        {
            get => _page[_layout.NextPageIndexOffset].Read<int>();
            set => _page[_layout.NextPageIndexOffset].Write(value);
        }

        private Span<byte> GetItemPointerOffset(int index)
        {
            var itemOffset = _layout.HeaderSize + _layout.ItemPointerSize * index;

            return _page[itemOffset].Slice(0, _layout.ItemPointerSize);
        }

        private (ushort offset, ushort length) GetItemPointer(int index)
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

        public delegate void ItemWriter(in Span<byte> destination);

        public byte AddItem(int length, ItemWriter writer)
        {
            // find last item offset
            int lastOffset;
            if (ItemCount == 0)
            {
                lastOffset = PageLayout.PageSize - 1;
            }
            else
            {
                (lastOffset, _) = GetItemPointer(ItemCount - 1);
            }

            var offset = lastOffset - length;
            var index = ItemCount;

            var pointerOffset = GetItemPointerOffset(index);
            pointerOffset.Write((ushort)offset);
            pointerOffset.Slice(_layout.ItemPointerOffsetSize).Write((ushort)length);

            var itemPointer = _page[offset].Slice(0, length);
            writer(itemPointer);

            return ItemCount++;
        }

        public void RemoveItem(byte index)
        {
            var (_, length) = GetItemPointer(index);

            for (var i = index + 1; i < ItemCount; i++)
            {
                var (ioffset, ilength) = GetItemPointer(i);

                if (ilength > length)
                {
                    throw new NotImplementedException("TODO handle overlapping copies");
                }

                // move the item forward by "length"
                _page[ioffset].Slice(0, ilength).CopyTo(_page[ioffset + length]);
            }

            for (var i = index + 1; i < ItemCount; i++)
            {
                GetItemPointerOffset(i).CopyTo(GetItemPointerOffset(i - 1));
            }

            ItemCount--;
        }

        public bool CanInsert(int itemSize)
        {
            var maxAvailableSpace = PageLayout.PageSize - _layout.HeaderSize - _layout.ItemPointerSize;
            if (maxAvailableSpace < itemSize)
            {
                throw new InvalidOperationException("Item will not fit on one page.");
            }

            if (ItemCount == 0)
            {
                return true;
            }

            var (lastOffset, _) = GetItemPointer(ItemCount - 1);
            var availableSpace = lastOffset - _layout.HeaderSize - (_layout.ItemPointerSize + 1) * ItemCount;

            return availableSpace >= itemSize;
        }
    }
}
