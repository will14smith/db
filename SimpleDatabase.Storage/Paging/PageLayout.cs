namespace SimpleDatabase.Storage.Paging
{
    public class PageLayout
    {
        public const int PageSize = 4096;

        public const int PageTypeSize = sizeof(PageType);
        public const int PageTypeOffset = 0;
    }
}