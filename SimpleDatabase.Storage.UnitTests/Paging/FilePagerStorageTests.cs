using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using SimpleDatabase.Storage.Paging;
using Xunit;

namespace SimpleDatabase.Storage.UnitTests.Paging
{
    public class FilePagerStorageTests
    {
        private readonly IFileSystem _fileSystem = new MockFileSystem();
        private readonly PageSource _source;
        private readonly string _file;

        public FilePagerStorageTests()
        {
            _source = new PageSource.Table("test");
            _file = _fileSystem.Path.GetTempFileName();
        }

        [Fact]

        public void Source_ShouldEqualConstructor()
        {
            using var storage = new FilePageStorage(_fileSystem, _source, _file);
            Assert.Equal(storage.Source, _source);
        }

        [Fact]

        public void Read_FromDifferentSource_ShouldThrow()
        {
            using var storage = new FilePageStorage(_fileSystem, _source, _file);
            Assert.ThrowsAny<Exception>(() => storage.Read(new PageId(new PageSource.Table("xyz"), 0)));
        }
        [Fact]

        public void Write_ToDifferentSource_ShouldThrow()
        {
            using var storage = new FilePageStorage(_fileSystem, _source, _file);
            var id = new PageId(new PageSource.Table("xyz"), 0);
            var page = new Page(id, new byte[PageLayout.PageSize]);

            Assert.ThrowsAny<Exception>(() => storage.Write(page));
        }
        [Fact]

        public void Write_WithWrongSize_ShouldThrow()
        {
            using var storage = new FilePageStorage(_fileSystem, _source, _file);
            var id = new PageId(_source, 0);
            var page = new Page(id, new byte[1]);

            Assert.ThrowsAny<Exception>(() => storage.Write(page));
        }


        [Fact]
        public void WriteThenRead_ShouldReturnOriginalData()
        {
            var page = CreatePage(0);

            using var storage = new FilePageStorage(_fileSystem, _source, _file);
            storage.Write(page);
            var result = storage.Read(page.Id);

            Assert.Equal(page.Data, result.Data);
        }

        [Fact]
        public void SameFileSecondInstance_ShouldReadOriginalData()
        {
            var page = CreatePage(0);

            using (var storage = new FilePageStorage(_fileSystem, _source, _file))
            {
                storage.Write(page);
            }
            using (var storage = new FilePageStorage(_fileSystem, _source, _file))
            {
                var result = storage.Read(page.Id);

                Assert.Equal(page.Data, result.Data);
            }
        }

        [Fact]
        public void PageCount_WithSinglePage_ShouldBeCorrect()
        {
            var page = CreatePage(0);

            using var storage = new FilePageStorage(_fileSystem, _source, _file);
            storage.Write(page);

            Assert.Equal(1, storage.PageCount);
        }
        [Fact]
        public void PageCount_WithMultiplePages_ShouldBeCorrect()
        {
            using var storage = new FilePageStorage(_fileSystem, _source, _file);
            storage.Write(CreatePage(0));
            storage.Write(CreatePage(1));
            storage.Write(CreatePage(2));

            Assert.Equal(3, storage.PageCount);
        }

        private Page CreatePage(int index)
        {
            var data = new byte[PageLayout.PageSize];

            for (var i = 0; i < data.Length; i++)
            {
                data[i] = (byte)i;
            }

            return new Page(new PageId(_source, index), data);
        }
    }
}
