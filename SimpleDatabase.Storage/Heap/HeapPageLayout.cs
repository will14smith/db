using SimpleDatabase.Storage.Paging;

namespace SimpleDatabase.Storage.Heap
{
    public class HeapPageLayout : PageLayout
    {
        // header
        public int ItemCountSize => sizeof(byte);
        public int PaddingSize => 2 * sizeof(byte);

        public int ItemCountOffset => PageTypeOffset + PageTypeSize;
        public int PaddingOffset => ItemCountOffset + ItemCountSize;

        public int HeaderSize => PageTypeSize + ItemCountSize + PaddingSize;

        // item pointers array
        public int ItemPointerOffsetSize => sizeof(ushort);
        public int ItemPointerLengthSize => sizeof(ushort);

        public int ItemPointerSize => ItemPointerOffsetSize + ItemPointerLengthSize;

        public int ItemPointersArrayOffset => HeaderSize;
    }
}