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
}
