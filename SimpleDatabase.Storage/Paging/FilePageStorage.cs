using System;
using System.IO;
using System.IO.Abstractions;

namespace SimpleDatabase.Storage.Paging
{
    public class FilePageStorage : IPageStorage
    {
        private FileSystemStream? _file;

        public FilePageStorage(IFileSystem fileSystem, PageSource source, string path)
        {
            Source = source;
            _file = fileSystem.File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            if (_file.Length % PageLayout.PageSize != 0)
            {
                throw new InvalidOperationException("paged file isn't a whole number of pages long");
            }
        }

        public PageSource Source { get; }
        public int PageCount => (int)_file!.Length / PageLayout.PageSize;

        public Page Read(PageId id)
        {
            if (!Equals(id.Source, Source))
            {
                throw new InvalidOperationException("requested source doesn't match");
            }

            var page = new byte[PageLayout.PageSize];

            var offset = id.Index * PageLayout.PageSize;
            _file!.Seek(offset, SeekOrigin.Begin);

            var bytesRead = _file!.Read(page, 0, PageLayout.PageSize);
            if (bytesRead != PageLayout.PageSize)
            {
                throw new IOException($"failed to read {PageLayout.PageSize} bytes, read {bytesRead} instead");
            }

            return new Page(id, page);
        }

        public void Write(Page page)
        {
            if (page.Data.Length != PageLayout.PageSize)
            {
                throw new ArgumentException($"Page size is {page.Data.Length} but should be {PageLayout.PageSize}", nameof(page));
            }

            var id = page.Id;
            if (!Equals(id.Source, Source))
            {
                throw new InvalidOperationException("requested source doesn't match");
            }

            var offset = id.Index * PageLayout.PageSize;
            _file!.Seek(offset, SeekOrigin.Begin);

            _file!.Write(page.Data, 0, PageLayout.PageSize);
        }

        public void Dispose()
        {
            if (_file == null)
            {
                return;
            }

            _file.Flush();
            _file.Dispose();
            _file = null;
        }
    }
}