namespace SimpleDatabase.Core.Paging
{
    public class Page
    {
        public Page(int number, byte[] data)
        {
            Number = number;
            Data = data;
        }

        public int Number { get; }
        public byte[] Data { get; }
    }
}