using Moq;
using SimpleDatabase.Core.Paging;
using SimpleDatabase.Core.Trees;
using Xunit;

namespace SimpleDatabase.Core.UnitTests.Paging
{
    public class PagerTests
    {
        [Theory]
        [InlineData(0, 0 * Row.RowSize)]
        [InlineData(10, 10 * Row.RowSize)]
        public void RowCount_ShouldBeExpectedValue(int expectedRowCount, int byteLength)
        {
            var (pager, _) = CreatePager(byteLength);

            var actualRowCount = pager.RowCount;

            Assert.Equal(expectedRowCount, actualRowCount);
        }

        [Fact]
        public void Get_WhenPageIndexIsInStorage_ShouldGetFromStorage()
        {
            var (pager, storage) = CreatePager(Pager.PageSize);

            pager.Get(0);

            storage.Verify(x => x.Read(0), Times.Once);
        }

        [Fact]
        public void Get_WhenPageIndexIsInStorage_ShouldCacheGetFromStorage()
        {
            var (pager, storage) = CreatePager(Pager.PageSize);
            pager.Get(0);
            storage.Reset();

            pager.Get(0);

            storage.Verify(x => x.Read(0), Times.Never);
        }

        [Fact]
        public void Get_WhenPageIndexOutsideStorage_ShouldNotGetFromStorage()
        {
            var (pager, storage) = CreatePager(0);

            pager.Get(0);

            storage.Verify(x => x.Read(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void Flush_WhenPageIsLoaded_ShouldWriteToStorage()
        {
            var (pager, storage) = CreatePager(0);
            pager.Get(0);

            pager.Flush(0);

            storage.Verify(x => x.Write(It.IsAny<Page>(), 0), Times.Once);
        }

        [Fact]
        public void Flush_WhenPageIsNotLoaded_ShouldNotWriteToStorage()
        {
            var (pager, storage) = CreatePager(0);

            pager.Flush(0);

            storage.Verify(x => x.Write(It.IsAny<Page>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void Dispose_ShouldFlushAllPagesInMemory()
        {
            var (pager, storage) = CreatePager(0);
            pager.Get(0);
            pager.Get(1);

            pager.Dispose();

            storage.Verify(x => x.Write(It.IsAny<Page>(), It.IsAny<int>()), Times.Exactly(2));
        }

        private (Pager, Mock<IPagerStorage>) CreatePager(int byteLength)
        {
            var storage = new Mock<IPagerStorage>();
            var pager = new Pager(storage.Object);

            storage.SetupGet(x => x.ByteLength).Returns(byteLength);

            return (pager, storage);
        }
    }
}
