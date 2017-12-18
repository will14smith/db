namespace SimpleDatabase.Storage.Paging
{
    public class PageId
    {
        public PageId(PageSource source, int index)
        {
            Source = source;
            Index = index;
        }

        public PageSource Source { get; }
        public int Index { get; }
    }
}