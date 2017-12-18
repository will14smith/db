using System;
using System.Collections.Generic;

namespace SimpleDatabase.Storage.Paging
{
    public class Pager : IPager
    {
        private readonly IPageStorageFactory _storageFactory;

        private readonly Dictionary<PageSource, IPageStorage> _storage = new Dictionary<PageSource, IPageStorage>();
        private readonly Dictionary<PageId, Page> _pages = new Dictionary<PageId, Page>();

        public Pager(IPageStorageFactory storageFactory)
        {
            _storageFactory = storageFactory;
        }

        public Page Get(PageId id)
        {
            var storage = GetStorage(id.Source);

            if (_pages.TryGetValue(id, out var page))
            {
                return page;
            }

            if (id.Index >= storage.PageCount)
            {
                throw new NotImplementedException();
            }

            page = storage.Read(id);
            _pages.Add(id, page);

            return page;
        }

        public void Flush(PageId id)
        {
            var storage = GetStorage(id.Source);

            if (!_pages.TryGetValue(id, out var page))
            {
                throw new InvalidOperationException("Cannot flush page that hasn't been loaded...");
            }

            storage.Write(page);
        }

        public Page Allocate(PageSource source)
        {
            var storage = GetStorage(source);

            // TODO check free list
            var unusedIndex = storage.PageCount;
            return Get(new PageId(source, unusedIndex));
        }

        public void Free(PageId id)
        {
            var storage = GetStorage(id.Source);

            // TODO implement a free list
        }

        private IPageStorage GetStorage(PageSource source)
        {
            if (_storage.TryGetValue(source, out var storage))
            {
                return storage;
            }

            storage = _storageFactory.Create(source);
            _storage.Add(source, storage);

            return storage;
        }

        public void Dispose()
        {
            // TODO Flush all?

            foreach (var s in _storage)
            {
                s.Value.Dispose();
            }
        }
    }
}
