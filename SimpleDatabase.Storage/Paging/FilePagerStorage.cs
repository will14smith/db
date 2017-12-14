using System;
using System.IO;

namespace SimpleDatabase.Storage.Paging
{
    public class FilePagerStorage : IPagerStorage
    {
        private FileStream _file;

        public FilePagerStorage(string path)
        {
            _file = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        }

        public int ByteLength => (int)_file.Length;

        public Page Read(PageId id)
        {
            if (id.StorageType != PageStorageType.Tree)
            {
                throw new NotImplementedException();
            }

            var page = new byte[PageLayout.PageSize];

            _file.Seek(id.Index * PageLayout.PageSize, SeekOrigin.Begin);
            // TODO check bytes read
            _file.Read(page, 0, PageLayout.PageSize);

            return new Page(id, page);
        }

        public void Write(Page page)
        {
            if (page.Id.StorageType != PageStorageType.Tree)
            {
                throw new NotImplementedException();
            }

            _file.Seek(page.Id.Index * PageLayout.PageSize, SeekOrigin.Begin);
            _file.Write(page.Data, 0, PageLayout.PageSize);
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