using SimpleDatabase.Core.Paging;

namespace SimpleDatabase.Core.Trees
{
    public static class NodeLayout
    {
        // Common Node Header
        public const int NodeTypeSize = sizeof(NodeType);
        public const int IsRootSize = sizeof(bool);
        public const int ParentPointerSize = sizeof(int);

        public const int NodeTypeOffset = 0;
        public const int IsRootOffset = NodeTypeOffset + NodeTypeSize;
        public const int ParentPointerOffset = IsRootOffset + IsRootSize;

        public const int CommonNodeHeaderSize = NodeTypeSize + IsRootSize + ParentPointerSize;

        // Leaf Node Header
        public const int LeafNodeCellCountSize = sizeof(int);
        public const int LeafNodeNextLeafSize = sizeof(int);

        public const int LeafNodeCellCountOffset = CommonNodeHeaderSize;
        public const int LeafNodeNextLeafOffset = LeafNodeCellCountOffset + LeafNodeCellCountSize;

        public const int LeafNodeHeaderSize = CommonNodeHeaderSize + LeafNodeCellCountSize + LeafNodeNextLeafSize;

        // Leaf Node Body
        public const int LeafNodeKeySize = sizeof(int);
        public const int LeafNodeValueSize = Row.RowSize;
        public const int LeafNodeCellSize = LeafNodeKeySize + LeafNodeValueSize;

        public const int LeafNodeKeyOffset = 0;
        public const int LeafNodeValueOffset = LeafNodeKeyOffset + LeafNodeKeySize;

        public const int LeafNodeSpaceForCells = Pager.PageSize - LeafNodeHeaderSize;
        public const int LeafNodeMaxCells = LeafNodeSpaceForCells / LeafNodeCellSize;
        public const int LeafNodeRightSplitCount = (LeafNodeMaxCells + 1) / 2;
        public const int LeafNodeLeftSplitCount = (LeafNodeMaxCells + 1) - LeafNodeRightSplitCount;

        // Internal Node Header
        public const int InternalNodeKeyCountSize = sizeof(int);
        public const int InternalNodeRightChildSize = sizeof(int);

        public const int InternalNodeKeyCountOffset = CommonNodeHeaderSize;
        public const int InternalNodeRightChildOffset = InternalNodeKeyCountOffset + InternalNodeKeyCountSize;

        public const int InternalNodeHeaderSize = CommonNodeHeaderSize + InternalNodeKeyCountSize + InternalNodeRightChildSize;

        // Internal Node Body
        public const int InternalNodeKeySize = sizeof(int);
        public const int InternalNodeChildSize = sizeof(int);

        public const int InternalNodeCellSize = InternalNodeChildSize + InternalNodeKeySize;


    }
}