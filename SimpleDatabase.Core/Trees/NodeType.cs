using SimpleDatabase.Core.Paging;

namespace SimpleDatabase.Core.Trees
{
    public enum NodeType : byte
    {
        Internal,
        Leaf
    }

    public static class NodeLayout
    {
        // Common Node Header
        public const int NodeTypeSize = sizeof(NodeType);
        public const int IsRootSize = sizeof(bool);
        public const int ParentPointer = sizeof(int);

        public const int NodeTypeOffset = 0;
        public const int IsRootOffset = NodeTypeOffset + NodeTypeSize;
        public const int ParentPointerOffset = IsRootOffset + IsRootSize;

        public const int CommonNodeHeaderSize = NodeTypeSize + IsRootSize + ParentPointer;

        // Leaf Node Header
        public const int LeafNodeCellCountSize = sizeof(int);

        public const int LeafNodeCellCountOffset = CommonNodeHeaderSize;

        public const int LeafNodeHeaderSize = CommonNodeHeaderSize + LeafNodeCellCountSize;

        // Leaf Node Body Layout
        public const int LeafNodeKeySize = sizeof(int);
        public const int LeafNodeValueSize = Row.RowSize;
        public const int LeafNodeCellSize = LeafNodeKeySize + LeafNodeValueSize;

        public const int LeafNodeKeyOffset = 0;
        public const int LeafNodeValueOffset = LeafNodeKeyOffset + LeafNodeKeySize;

        public const int LeafNodeSpaceForCells = Pager.PageSize - LeafNodeHeaderSize;
        public const int LeafNodeMaxCells = LeafNodeSpaceForCells / LeafNodeCellSize;


    }
}
