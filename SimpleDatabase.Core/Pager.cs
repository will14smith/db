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

    public interface IPagerStorage : IDisposable
    {
        int ByteLength { get; }

        Page Read(int index);
        void Write(Page page, int index, int size);
    }

    public class FilePagerStorage : IPagerStorage
    {
        private readonly FileStream _file;

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

            return new Page(page);
        }

        public void Write(Page page, int index, int size)
        {
            _file.Seek(index * Pager.PageSize, SeekOrigin.Begin);
            _file.Write(page.Data, 0, size);
        }

        public void Dispose()
        {
            _file?.Dispose();
        }
    }

    public class Pager : IPager
    {
        public const int PageSize = 4096;
        public const int MaxPages = 100;
        public const int RowsPerPage = PageSize / Row.RowSize;
        public const int MaxRows = RowsPerPage * MaxPages;

        private readonly IPagerStorage _storage;
        private readonly Page[] _pages = new Page[MaxPages];

        public Pager(IPagerStorage storage)
        {
            _storage = storage;
        }

        public int RowCount => _storage.ByteLength / Row.RowSize;

        public Page Get(int index)
        {
            if (index > MaxPages)
            {
                throw new ArgumentOutOfRangeException(nameof(index), $"Index was larger than the number of allowed pages ({MaxPages})");
            }

            if (_pages[index] == null)
            {

                var pageCountInStorage = _storage.ByteLength / PageSize;
                // last page might not be completely full
                if (_storage.ByteLength % PageSize != 0)
                {
                    pageCountInStorage += 1;
                }

                if (index < pageCountInStorage)
                {
                    _pages[index] = _storage.Read(index);
                }
                else
                {
                    _pages[index] = new Page(new byte[PageSize]);
                }
            }

            return _pages[index];
        }

        public void Flush(int index, int pageSize)
        {
            if (_pages[index] == null)
            {
                return;
            }

            _storage.Write(_pages[index], index, pageSize);
        }

        public void Evict(int index, int pageSize)
        {
            Flush(index, pageSize);
            _pages[index] = null;
        }

        public void Dispose()
        {
            _storage?.Dispose();
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
