namespace SimpleDatabase.Storage.Paging
{
    public interface ISourcePager
    {
        PageSource Source { get; }

        Page Get(int index);
        void Flush(int index);

        Page Allocate();
        void Free(int index);
    }

    public class SourcePager : ISourcePager
    {
        private readonly IPager _pager;

        public PageSource Source { get; }

        public SourcePager(IPager pager, PageSource source)
        {
            _pager = pager;
            Source = source;
        }

        public Page Get(int index)
        {
            return _pager.Get(new PageId(Source, index));
        }

        public void Flush(int index)
        {
            _pager.Flush(new PageId(Source, index));
        }

        public Page Allocate()
        {
            return _pager.Allocate(Source);
        }

        public void Free(int index)
        {
            _pager.Free(new PageId(Source, index));
        }
    }
}
