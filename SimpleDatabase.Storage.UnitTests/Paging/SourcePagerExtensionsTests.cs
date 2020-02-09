using System;
using Moq;
using SimpleDatabase.Storage.Paging;
using Xunit;

namespace SimpleDatabase.Storage.UnitTests.Paging
{
    public class SourcePagerExtensionsTests
    {
        [Fact]
        public void Get_DifferentSource_ShouldThrow()
        {
            var pager = Mock.Of<ISourcePager>(x => x.Source == new PageSource.Heap("a"));

            Assert.ThrowsAny<Exception>(() => pager.Get(new PageId(new PageSource.Heap("b"), 1)));
        }

        [Fact]
        public void Get_ShouldPassIndex()
        {
            var source = new PageSource.Heap("a");
            var index = 1;
            var pager = Mock.Of<ISourcePager>(x => x.Source == source);

            pager.Get(new PageId(source, index));

            Mock.Get(pager).Verify(x => x.Get(index), Times.Once);
        }

        [Fact]
        public void Flush_DifferentSource_ShouldThrow()
        {
            var pager = Mock.Of<ISourcePager>(x => x.Source == new PageSource.Heap("a"));

            Assert.ThrowsAny<Exception>(() => pager.Flush(new PageId(new PageSource.Heap("b"), 1)));
        }

        [Fact]
        public void Flush_ShouldPassIndex()
        {
            var source = new PageSource.Heap("a");
            var index = 1;
            var pager = Mock.Of<ISourcePager>(x => x.Source == source);

            pager.Flush(new PageId(source, index));

            Mock.Get(pager).Verify(x => x.Flush(index), Times.Once);
        }

        [Fact]
        public void Free_DifferentSource_ShouldThrow()
        {
            var pager = Mock.Of<ISourcePager>(x => x.Source == new PageSource.Heap("a"));

            Assert.ThrowsAny<Exception>(() => pager.Free(new PageId(new PageSource.Heap("b"), 1)));
        }

        [Fact]
        public void Free_ShouldPassIndex()
        {
            var source = new PageSource.Heap("a");
            var index = 1;
            var pager = Mock.Of<ISourcePager>(x => x.Source == source);

            pager.Free(new PageId(source, index));

            Mock.Get(pager).Verify(x => x.Free(index), Times.Once);
        }
    }
}
