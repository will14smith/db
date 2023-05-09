using Moq;
using SimpleDatabase.Storage.Paging;
using Xunit;

namespace SimpleDatabase.Storage.UnitTests.Paging
{
    public class SourcePagerTests
    {
        [Fact]
        public void Source_ShouldBeSameAsConstructor()
        {
            var source = new PageSource.Table("a");
            var pager = new SourcePager(Mock.Of<IPager>(), source);

            Assert.Equal(source, pager.Source);
        }

        [Fact]
        public void Get_ShouldPassSourceAndIndex()
        {
            var source = new PageSource.Table("a");
            var index = 0;
            var pager = Mock.Of<IPager>();
            var sourcePager = new SourcePager(pager, source);

            sourcePager.Get(index);

            Mock.Get(pager).Verify(x => x.Get(new PageId(source, index)), Times.Once);
        }

        [Fact]
        public void Flush_ShouldPassSourceAndIndex()
        {
            var source = new PageSource.Table("a");
            var index = 0;
            var pager = Mock.Of<IPager>();
            var sourcePager = new SourcePager(pager, source);

            sourcePager.Flush(index);

            Mock.Get(pager).Verify(x => x.Flush(new PageId(source, index)), Times.Once);
        }

        [Fact]
        public void Alloc_ShouldPassSource()
        {
            var source = new PageSource.Table("a");
            var pager = Mock.Of<IPager>();
            var sourcePager = new SourcePager(pager, source);

            sourcePager.Allocate();

            Mock.Get(pager).Verify(x => x.Allocate(source), Times.Once);
        }

        [Fact]
        public void Free_ShouldPassSourceAndIndex()
        {
            var source = new PageSource.Table("a");
            var index = 0;
            var pager = Mock.Of<IPager>();
            var sourcePager = new SourcePager(pager, source);

            sourcePager.Free(index);

            Mock.Get(pager).Verify(x => x.Free(new PageId(source, index)), Times.Once);
        }
    }
}
