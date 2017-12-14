namespace SimpleDatabase.Storage.Paging
{
    public class PageLayout
    {
        public const int PageSize = 4096;

        public static readonly int PageTypeSize = sizeof(PageType);
        public static readonly int PageTypeOffset = 0;
    }
}