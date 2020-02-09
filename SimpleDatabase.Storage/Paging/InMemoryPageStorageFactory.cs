namespace SimpleDatabase.Storage.Paging
{
    public class InMemoryPageStorageFactory : IPageStorageFactory
    {
        public IPageStorage Create(PageSource source)
        {
            return new InMemoryPageStorage(source);
        }
    }
}
