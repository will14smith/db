namespace SimpleDatabase.Storage.Paging
{
    public struct PageId
    {
        public PageId(PageStorageType storageType, int index)
        {
            StorageType = storageType;
            Index = index;
        }

        public PageStorageType StorageType { get; }
        public int Index { get; }
    }
}