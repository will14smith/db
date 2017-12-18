using SimpleDatabase.Storage.Paging;
using Xunit;

namespace SimpleDatabase.Storage.UnitTests.Paging
{
    public class FolderPageSourceFactoryTests
    {
        [Fact]
        public void FilePathToSource_WithHeap_ShouldReturnCorrectPath()
        {
            var source = new PageSource.Heap("table123");
            var factory = new FolderPageSourceFactory(@"C:\database");

            var path = factory.FilePathToSource(source);

            Assert.Equal(@"C:\database\table123.tbh", path);
        }

        [Fact]
        public void FilePathToSource_WithIndex_ShouldReturnCorrectPath()
        {
            var source = new PageSource.Index("table123", "pk");
            var factory = new FolderPageSourceFactory(@"C:\database");

            var path = factory.FilePathToSource(source);

            Assert.Equal(@"C:\database\table123_pk.idx", path);
        }
    }
}
