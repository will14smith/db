using System;

namespace SimpleDatabase.Storage.Paging
{
    public class Pager : IPager
    {
        public const int MaxPages = 2000;

        private readonly IPagerStorage _storage;
        private readonly Page[] _pages = new Page[MaxPages];

        private int _pageCount;

        public Pager(IPagerStorage storage)
        {
            _storage = storage;

            _pageCount = _storage.ByteLength / PageLayout.PageSize;
            if (_storage.ByteLength % PageLayout.PageSize != 0)
            {
                throw new InvalidOperationException("storage does not have a whole number of pages");

            }

        }

        public Page Get(PageId id)
        {
            if (id.StorageType != PageStorageType.Tree)
            {
                throw new NotImplementedException();
            }

            var index = id.Index;
            if (index > MaxPages)
            {
                throw new ArgumentOutOfRangeException(nameof(index), $"Index was larger than the number of allowed pages ({MaxPages})");
            }

            if (_pages[index] == null)
            {
                // TODO check page is allocated
                _pages[index] = _storage.Read(id);

                if (index >= _pageCount)
                {
                    _pageCount = index + 1;
                }
            }

            return _pages[index];
        }

        public void Flush(PageId id)
        {
            if (id.StorageType != PageStorageType.Tree)
            {
                throw new NotImplementedException();
            }

            var index = id.Index;
            if (_pages[index] == null)
            {
                return;
            }

            _storage.Write(_pages[index]);
        }

        public Page Allocate(PageStorageType type)
        {
            if (type != PageStorageType.Tree)
            {
                throw new NotImplementedException();
            }

            // TODO check free list
            var unusedIndex = _pageCount;
            return Get(new PageId(type, unusedIndex));
        }

        public void Free(PageId id)
        {
            if (id.StorageType != PageStorageType.Tree)
            {
                throw new NotImplementedException();
            }

            // TODO implement a free list
        }

        public void Dispose()
        {
            for (var i = 0; i < _pageCount; i++)
            {
                Flush(new PageId(PageStorageType.Tree, i));
                _pages[i] = null;
            }

            _storage?.Dispose();
        }
    }
}
