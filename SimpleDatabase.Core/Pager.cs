using System;
using System.IO;

namespace SimpleDatabase.Core
{
    public interface IPager : IDisposable
    {
        int RowCount { get; }

        Page Get(int index);
        void Flush(int index, int pageSize);
        void Evict(int index, int pageSize);
    }

    public class Pager : IPager
    {
        public const int PageSize = 4096;
        public const int MaxPages = 100;
        public const int RowsPerPage = PageSize / Row.RowSize;
        public const int MaxRows = RowsPerPage * MaxPages;

        private readonly FileStream _file;
        private readonly Page[] _pages = new Page[MaxPages];

        public Pager(string path)
        {
            _file = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        }

        public int RowCount => (int) (_file.Length / Row.RowSize);

        public Page Get(int index)
        {
            if (index > MaxPages)
            {
                throw new ArgumentOutOfRangeException(nameof(index), $"Index was larger than the number of allowed pages ({MaxPages})");
            }

            if (_pages[index] == null)
            {
                var page = new byte[PageSize];

                var pageCountInFile = _file.Length / PageSize;
                // last page might not be completely full
                if (_file.Length % PageSize != 0)
                {
                    pageCountInFile += 1;
                }

                if (index < pageCountInFile)
                {
                    _file.Seek(index * PageSize, SeekOrigin.Begin);
                    // TODO check bytes read
                    _file.Read(page, 0, PageSize);
                }

                _pages[index] = new Page(page);
            }

            return _pages[index];
        }

        public void Flush(int index, int pageSize)
        {
            if (_pages[index] == null)
            {
                return;
            }

            _file.Seek(index * PageSize, SeekOrigin.Begin);
            _file.Write(_pages[index].Data, 0, pageSize);
        }

        public void Evict(int index, int pageSize)
        {
            Flush(index, pageSize);
            _pages[index] = null;
        }

        public void Dispose()
        {
            _file?.Dispose();
        }
    }

    public class Page
    {
        public Page(byte[] data)
        {
            Data = data;
        }

        public byte[] Data { get; }
    }
}
