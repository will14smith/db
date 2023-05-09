using System.IO.Abstractions.TestingHelpers;
using SimpleDatabase.Storage.Paging;
using Xunit;

namespace SimpleDatabase.Storage.UnitTests.Paging
{
    public class FolderPageSourceFactoryTests
    {
        [Fact]
        public void FilePathToSource_WithHeap_ShouldReturnCorrectPath()
        {
            var source = new PageSource.Table("table123");
            var factory = new FolderPageSourceFactory(new MockFileSystem(), @"C:\database");

            var path = factory.FilePathToSource(source);

            Assert.Equal(@"C:\database\table123.tbl", path);
        }

        [Fact]
        public void FilePathToSource_WithDatabase_ShouldReturnCorrectPath()
        {
            var source = PageSource.Database.Instance;
            var factory = new FolderPageSourceFactory(new MockFileSystem(), @"C:\database");

            var path = factory.FilePathToSource(source);

            Assert.Equal(@"C:\database\database", path);
        }
    }
}
