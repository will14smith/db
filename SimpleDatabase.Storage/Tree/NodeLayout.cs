using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;

namespace SimpleDatabase.Storage.Tree
{
    public class NodeLayout : PageLayout
    {
        private readonly IRowSerializer _rowSerializer;

        public NodeLayout(IRowSerializer rowSerializer)
        {
            _rowSerializer = rowSerializer;
        }

        // Common Node Header
        public readonly int IsRootSize = sizeof(bool);
        public readonly int IsRootOffset = PageTypeOffset + PageTypeSize;

        public int CommonNodeHeaderSize => PageTypeSize + IsRootSize;

        // Leaf Node Header
        public int LeafNodeCellCountSize => sizeof(int);
        public int LeafNodeNextLeafSize => sizeof(int);

        public int LeafNodeCellCountOffset => CommonNodeHeaderSize;
        public int LeafNodeNextLeafOffset => LeafNodeCellCountOffset + LeafNodeCellCountSize;

        public int LeafNodeHeaderSize => CommonNodeHeaderSize + LeafNodeCellCountSize + LeafNodeNextLeafSize;

        // Leaf Node Body
        public int LeafNodeKeySize = sizeof(int);
        public int LeafNodeValueSize => _rowSerializer.GetRowSize();
        public int LeafNodeCellSize => LeafNodeKeySize + LeafNodeValueSize;

        public int LeafNodeKeyOffset = 0;
        public int LeafNodeValueOffset => LeafNodeKeyOffset + LeafNodeKeySize;

        public int LeafNodeSpaceForCells => Pager.PageSize - LeafNodeHeaderSize;
        public int LeafNodeMaxCells => LeafNodeSpaceForCells / LeafNodeCellSize;
        public int LeafNodeMinCells => (LeafNodeMaxCells + 1) / 2 - 1; // ceil(MaxCells / 2) - 1
        public int LeafNodeRightSplitCount => LeafNodeMinCells;
        public int LeafNodeLeftSplitCount => LeafNodeMaxCells - LeafNodeRightSplitCount;

        // Internal Node Header
        public int InternalNodeKeyCountSize = sizeof(int);
        public int InternalNodeRightChildSize = sizeof(int);

        public int InternalNodeKeyCountOffset => CommonNodeHeaderSize;
        public int InternalNodeRightChildOffset => InternalNodeKeyCountOffset + InternalNodeKeyCountSize;

        public int InternalNodeHeaderSize => CommonNodeHeaderSize + InternalNodeKeyCountSize + InternalNodeRightChildSize;

        // Internal Node Body
        public int InternalNodeKeySize = sizeof(int);
        public int InternalNodeChildSize = sizeof(int);

        public int InternalNodeCellSize => InternalNodeChildSize + InternalNodeKeySize;

        public int InternalNodeSpaceForCells => Pager.PageSize - InternalNodeHeaderSize;
        public int InternalNodeMaxCells => InternalNodeSpaceForCells / InternalNodeCellSize;
        public int InternalNodeMinCells => InternalNodeMaxCells / 2 - 1;
        public int InternalNodeRightSplitCount => InternalNodeMinCells + 1;
        public int InternalNodeLeftSplitCount => InternalNodeMaxCells - InternalNodeRightSplitCount;

    }
}