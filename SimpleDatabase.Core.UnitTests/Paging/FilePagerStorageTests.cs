using System;
using System.IO;
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
            var page = 1;
            var data = GetData();

            using (var storage = new FilePagerStorage(_file))
            {
                storage.Write(data, page);
                var result = storage.Read(page);

                Assert.Equal(data.Data, result.Data);
            }
        }

        [Fact]
        public void SameFileSecondInstance_ShouldReadOriginalData()
        {
            var page = 1;
            var data = GetData();

            using (var storage = new FilePagerStorage(_file))
            {
                storage.Write(data, page);
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
            var page = 1;
            var data = GetData();

            var expectedSize = (page + 1) * Pager.PageSize;

            using (var storage = new FilePagerStorage(_file))
            {
                storage.Write(data, page);

                Assert.Equal(expectedSize, storage.ByteLength);
            }
        }
        [Fact]
        public void ByteLengthOnSecondInstance_ShouldBeFileLength()
        {
            var page = 1;
            var data = GetData();

            var expectedSize = (page + 1) * Pager.PageSize;

            using (var storage = new FilePagerStorage(_file))
            {
                storage.Write(data, page);
            }
            using (var storage = new FilePagerStorage(_file))
            {
                Assert.Equal(expectedSize, storage.ByteLength);
            }
        }

        private static Page GetData()
        {
            var page = new byte[Pager.PageSize];

            for (var i = 0; i < Pager.PageSize; i++)
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
