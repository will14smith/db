using System;
using SimpleDatabase.Storage.Heap;
using SimpleDatabase.Storage.Paging;
using Xunit;

namespace SimpleDatabase.Storage.UnitTests.Heap
{
    public class HeapPageTests
    {
        [Fact]
        public void New_ShouldSetInitialValues()
        {
            var page = CreatePage();

            var heap = HeapPage.New(page);

            Assert.Equal(PageType.Heap, heap.Type);
            Assert.Equal(0, heap.ItemCount);
            Assert.Equal(0, heap.NextPageIndex);
        }

        [Fact]
        public void Read_NonHeapPage_ShouldThrow()
        {
            var page = CreatePage();

            Assert.ThrowsAny<Exception>(() => HeapPage.Read(page));
        }

        [Fact]
        public void Type_ShouldReturnHeap()
        {
            var heap = CreateNewHeap();

            Assert.Equal(PageType.Heap, heap.Type);
        }

        [Fact]
        public void NextPageIndex_ShouldReturnValue()
        {
            var heap = CreateNewHeap();
            var nextPageIndex = 10;

            heap.NextPageIndex = nextPageIndex;

            Assert.Equal(nextPageIndex, heap.NextPageIndex);
        }

        [Fact]
        public void Add_ShouldReturnIndex()
        {
            var heap = CreateNewHeap();

            var index1 = heap.AddItem(10, (in Span<byte> dest) => dest[0] = 1);
            var index2 = heap.AddItem(3, (in Span<byte> dest) => dest[2] = 3);

            Assert.Equal(2, heap.ItemCount);
            Assert.Equal(0, index1);
            Assert.Equal(1, index2);
        }

        [Fact]
        public void Add_EmptyItem_ShouldAddItem()
        {
            var heap = CreateNewHeap();

            var index1 = heap.AddItem(0, (in Span<byte> dest) => { });

            Assert.Equal(1, heap.ItemCount);
            Assert.Equal(0, index1);
        }

        [Fact]
        public void Get_ShouldReturnItem()
        {
            var heap = CreateNewHeap();
            var index1 = heap.AddItem(10, (in Span<byte> dest) => dest[0] = 1);
            var index2 = heap.AddItem(3, (in Span<byte> dest) => dest[2] = 3);

            var item1 = heap.GetItem(index1);
            var item2 = heap.GetItem(index2);

            Assert.Equal(10, item1.Length);
            Assert.Equal(1, item1[0]);
            Assert.Equal(0, item1[2]);
            Assert.Equal(3, item2.Length);
            Assert.Equal(3, item2[2]);
        }

        [Fact]
        public void Remove_ShouldRemoveItem()
        {
            var heap = CreateNewHeap();
            heap.AddItem(10, (in Span<byte> dest) => dest[0] = 1);
            var index = heap.AddItem(3, (in Span<byte> dest) => dest[2] = 3);

            heap.RemoveItem(index);

            Assert.Equal(1, heap.ItemCount);
        }

        [Fact]
        public void Remove_ShouldShiftItems()
        {
            var heap = CreateNewHeap();
            heap.AddItem(10, (in Span<byte> dest) => dest[0] = 1);
            heap.AddItem(3, (in Span<byte> dest) => dest[2] = 3);

            heap.RemoveItem(0);

            var item = heap.GetItem(0);
            Assert.Equal(3, item.Length);
            Assert.Equal(3, item[2]);
        }

        [Fact]
        public void CanInsert_SmallItem_ShouldBeTrue()
        {
            var heap = CreateNewHeap();
            heap.AddItem(PageLayout.PageSize - 1000, (in Span<byte> dest) => dest[0] = 1);

            Assert.True(heap.CanInsert(100));
        }
        [Fact]
        public void CanInsert_LargeItem_ShouldBeFalse()
        {
            var heap = CreateNewHeap();
            heap.AddItem(PageLayout.PageSize - 1000, (in Span<byte> dest) => dest[0] = 1);

            Assert.False(heap.CanInsert(1000));
        }
        [Fact]
        public void CanInsert_ItemLargerThanPage_ShouldThrow()
        {
            var heap = CreateNewHeap();
            heap.AddItem(PageLayout.PageSize - 1000, (in Span<byte> dest) => dest[0] = 1);

            Assert.ThrowsAny<Exception>(() => heap.CanInsert(PageLayout.PageSize + 100));
        }

        private static Page CreatePage()
        {
            return new Page(new PageId(new PageSource.Heap("a"), 0), new byte[PageLayout.PageSize]);
        }
        private static HeapPage CreateNewHeap(Page? page = null)
        {
            return HeapPage.New(page ?? CreatePage());
        }
    }
}
