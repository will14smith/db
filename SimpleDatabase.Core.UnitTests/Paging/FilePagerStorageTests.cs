using System;
using System.IO;
using System.Linq;
using SimpleDatabase.Core.Paging;
using Xunit;

namespace SimpleDatabase.Core.UnitTests.Paging
{
    public class FilePagerStorageTests : IDisposable
    {
        private readonly string _file;

        public FilePagerStorageTests()
        {
            _file = Path.GetTempFileName();
        }

        [Fact]
        public void WriteThenRead_ShouldReturnOriginalData()
        {
            var size = Pager.PageSize;
            var page = 1;
            var data = GetData(size);

            using (var storage = new FilePagerStorage(_file))
            {
                storage.Write(data, page, size);
                var result = storage.Read(page);

                Assert.Equal(data.Data, result.Data);
            }
        }
        [Fact]
        public void WriteThenRead_WithPartialPage_ShouldReturnOriginalData()
        {
            var size = 10;
            var page = 1;
            var data = GetData(size);

            using (var storage = new FilePagerStorage(_file))
            {
                storage.Write(data, page, size);
                var result = storage.Read(page);

                Assert.Equal(data.Data, result.Data.Take(size));
            }
        }

        [Fact]
        public void SameFileSecondInstance_ShouldReadOriginalData()
        {
            var size = Pager.PageSize;
            var page = 1;
            var data = GetData(size);

            using (var storage = new FilePagerStorage(_file))
            {
                storage.Write(data, page, size);
            }
            using (var storage = new FilePagerStorage(_file))
            {
                var result = storage.Read(page);

                Assert.Equal(data.Data, result.Data);
            }
        }

        [Fact]
        public void ByteLength_ShouldBeFileLength()
        {
            var size = 10;
            var page = 1;
            var data = GetData(size);

            var expectedSize = page * Pager.PageSize + size;

            using (var storage = new FilePagerStorage(_file))
            {
                storage.Write(data, page, size);

                Assert.Equal(expectedSize, storage.ByteLength);
            }
        }
        [Fact]
        public void ByteLengthOnSecondInstance_ShouldBeFileLength()
        {
            var size = 10;
            var page = 1;
            var data = GetData(size);

            var expectedSize = page * Pager.PageSize + size;

            using (var storage = new FilePagerStorage(_file))
            {
                storage.Write(data, page, size);
            }
            using (var storage = new FilePagerStorage(_file))
            {
                Assert.Equal(expectedSize, storage.ByteLength);
            }
        }

        private static Page GetData(int length)
        {
            var page = new byte[length];

            for (var i = 0; i < length; i++)
            {
                page[i] = (byte)i;
            }

            return new Page(page);
        }

        public void Dispose()
        {
            File.Delete(_file);
        }
    }
}
