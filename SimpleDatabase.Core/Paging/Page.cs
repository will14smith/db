namespace SimpleDatabase.Core.Paging
{
    public class Page
    {
        public Page(byte[] data)
        {
            Data = data;
        }

        public byte[] Data { get; }
    }
}