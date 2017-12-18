namespace SimpleDatabase.Storage.Paging
{
    public interface IPageStorageFactory
    {
        IPageStorage Create(PageSource source);
    }
}
