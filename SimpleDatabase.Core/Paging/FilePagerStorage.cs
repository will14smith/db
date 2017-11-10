using System.IO;

namespace SimpleDatabase.Core.Paging
{
    public class FilePagerStorage : IPagerStorage
    {
        private FileStream _file;

        public FilePagerStorage(string path)
        {
            _file = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        }

        public int ByteLength => (int) _file.Length;

        public Page Read(int index)
        {
            var page = new byte[Pager.PageSize];

            _file.Seek(index * Pager.PageSize, SeekOrigin.Begin);
            // TODO check bytes read
            _file.Read(page, 0, Pager.PageSize);

            return new Page(index, page);
        }

        public void Write(Page page, int index)
        {
            _file.Seek(index * Pager.PageSize, SeekOrigin.Begin);
            _file.Write(page.Data, 0, Pager.PageSize);
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