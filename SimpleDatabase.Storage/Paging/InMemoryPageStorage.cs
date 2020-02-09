using System;
using System.Collections.Generic;

namespace SimpleDatabase.Storage.Paging
{
    public class InMemoryPageStorage : IPageStorage
    {
        private readonly List<byte[]> _pages = new List<byte[]>();

        public InMemoryPageStorage(PageSource source)
        {
            Source = source;
        }

        public PageSource Source { get; }
        public int PageCount => _pages.Count;

        public Page Read(PageId id)
        {
            if (!Equals(id.Source, Source))
            {
                throw new InvalidOperationException("requested source doesn't match");
            }

            var page = _pages[id.Index];
            var copyOfPage = (byte[])page.Clone();

            return new Page(id, copyOfPage);
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

            while (id.Index >= _pages.Count)
            {
                _pages.Add(new byte[PageLayout.PageSize]);
            }

            _pages[id.Index] = page.Data;
        }

        public void Dispose()
        {
        }
    }
}