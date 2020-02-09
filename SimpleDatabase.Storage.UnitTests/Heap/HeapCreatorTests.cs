using System;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Heap;
using SimpleDatabase.Storage.Paging;
using Xunit;

namespace SimpleDatabase.Storage.UnitTests.Heap
{
    public class HeapCreatorTests
    {
        private readonly Table _table = new Table("a", new Column[0], new Index[0]);

        [Fact]
        public void AllocateReturnsIndex_ShouldThrow()
        {
            var pager = new SourcePager(CreatePager(), new PageSource.Index(_table.Name, "a"));
            var creator = new HeapCreator(pager, _table);

            Assert.ThrowsAny<Exception>(() => creator.Create());
        }
        [Fact]
        public void AllocateReturnsDifferentHeap_ShouldThrow()
        {
            var pager = new SourcePager(CreatePager(), new PageSource.Heap("b"));
            var creator = new HeapCreator(pager, _table);

            Assert.ThrowsAny<Exception>(() => creator.Create());
        }
        [Fact]
        public void AlreadyCreated_ShouldThrow()
        {
            var pager = new SourcePager(CreatePager(), new PageSource.Heap(_table.Name));
            var creator = new HeapCreator(pager, _table);

            creator.Create();

            Assert.ThrowsAny<Exception>(() => creator.Create());
        }

        [Fact]
        public void ShouldInitializePage()
        {
            var sourcePager = new SourcePager(CreatePager(), new PageSource.Heap(_table.Name));
            var creator = new HeapCreator(sourcePager, _table);

            creator.Create();

            var page = HeapPage.Read(sourcePager.Get(0));
            Assert.Equal(PageType.Heap, page.Type);
        }

        private static IPager CreatePager()
        {
            return new Pager(new InMemoryPageStorageFactory());
        }
    }
}
