using SimpleDatabase.Storage.Paging;

namespace SimpleDatabase.Storage.Heap
{
    public class HeapPageLayout : PageLayout
    {
        // header
        public int ItemCountSize => sizeof(byte);
        public int NextPageIndexSize => sizeof(int);

        public int ItemCountOffset => PageTypeOffset + PageTypeSize;
        public int NextPageIndexOffset => ItemCountOffset + ItemCountSize;

        public int HeaderSize => PageTypeSize + ItemCountSize + NextPageIndexSize;

        // item pointers array
        public int ItemPointerOffsetSize => sizeof(ushort);
        public int ItemPointerLengthSize => sizeof(ushort);

        public int ItemPointerSize => ItemPointerOffsetSize + ItemPointerLengthSize;

        public int ItemPointersArrayOffset => HeaderSize;
    }
}