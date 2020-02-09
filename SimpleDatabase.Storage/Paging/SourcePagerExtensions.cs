using System;

namespace SimpleDatabase.Storage.Paging
{
    public static class SourcePagerExtensions
    {
        public static Page Get(this ISourcePager pager, PageId index)
        {
            if (index.Source != pager.Source)
            {
                throw new InvalidOperationException();
            }

            return pager.Get(index.Index);
        }

        public static void Flush(this ISourcePager pager, PageId index)
        {
            if (index.Source != pager.Source)
            {
                throw new InvalidOperationException();
            }

            pager.Flush(index.Index);
        }

        public static void Free(this ISourcePager pager, PageId index)
        {
            if (index.Source != pager.Source)
            {
                throw new InvalidOperationException();
            }

            pager.Free(index.Index);
        }
    }
}