using System;

namespace SimpleDatabase.Core.Paging
{
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

        public void Dispose()
        {
            _storage?.Dispose();
        }
    }
}
