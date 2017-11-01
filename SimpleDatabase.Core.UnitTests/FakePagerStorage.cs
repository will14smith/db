using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleDatabase.Core.UnitTests
{
    class FakePagerStorage : IPagerStorage
    {
        private readonly IDictionary<int, byte[]> _pages = new Dictionary<int, byte[]>();

        public int ByteLength { get; private set; }

        public Page Read(int index)
        {
            if (!_pages.TryGetValue(index, out var page))
            {
                page = new byte[Pager.PageSize];
                _pages.Add(index, page);
            }

            var copyOfPage = page.ToArray();
            return new Page(copyOfPage);
        }

        public void Write(Page page, int index, int size)
        {
            _pages[index] = page.Data;

            var endOfPage = index * Pager.PageSize + size;
            if (endOfPage > ByteLength)
            {
                ByteLength = endOfPage;
            }
        }

        public void Dispose()
        {
        }
    }
}
