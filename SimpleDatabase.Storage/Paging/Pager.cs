using System;

namespace SimpleDatabase.Storage.Paging
{
    public class Pager : IPager
    {
        public const int PageSize = 4096;
        public const int MaxPages = 2000;

        private readonly IPagerStorage _storage;
        private readonly Page[] _pages = new Page[MaxPages];

        public Pager(IPagerStorage storage)
        {
            _storage = storage;

            PageCount = _storage.ByteLength / PageSize;
            if (_storage.ByteLength % PageSize != 0)
            {
                throw new InvalidOperationException("storage does not have a whole number of pages");

            }

        }

        public int PageCount { get; private set; }

        public Page Get(int index)
        {
            if (index > MaxPages)
            {
                throw new ArgumentOutOfRangeException(nameof(index), $"Index was larger than the number of allowed pages ({MaxPages})");
            }

            if (_pages[index] == null)
            {
                // TODO check page is allocated
                _pages[index] = _storage.Read(index);

                if (index >= PageCount)
                {
                    PageCount = index + 1;
                }
            }

            return _pages[index];
        }

        public void Flush(int index)
        {
            if (_pages[index] == null)
            {
                return;
            }

            _storage.Write(_pages[index], index);
        }

        public Page Allocate()
        {
            // TODO check free list
            var unusedIndex = PageCount;
            return Get(unusedIndex);
        }

        public void Free(int index)
        {
            // TODO implement a free list
        }

        public void Dispose()
        {
            for (var i = 0; i < PageCount; i++)
            {
                Flush(i);
                _pages[i] = null;
            }

            _storage?.Dispose();
        }
    }
}
