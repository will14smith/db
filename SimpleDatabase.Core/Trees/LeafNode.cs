using System;
using SimpleDatabase.Core.Paging;

namespace SimpleDatabase.Core.Trees
{
    public class LeafNode : Node
    {
        private LeafNode(Page page) : base(page)
        {
        }

        public static LeafNode New(Page page)
        {
            var node = new LeafNode(page)
            {
                Type = NodeType.Leaf,
                CellCount = 0
            };

            return node;
        }

        public static LeafNode Read(Page page)
        {
            return new LeafNode(page);
        }

        public int CellCount
        {
            get => BitConverter.ToInt32(Page.Data, NodeLayout.LeafNodeCellCountOffset);
            protected set => BitConverter.GetBytes(value).CopyTo(Page.Data, NodeLayout.LeafNodeCellCountOffset);
        }

        public int GetCellKeyOffset(int index)
        {
            return NodeLayout.LeafNodeHeaderSize + index * NodeLayout.LeafNodeCellCountSize;
        }
        public int GetCellValueOffset(int index)
        {
            return NodeLayout.LeafNodeHeaderSize + index * NodeLayout.LeafNodeCellCountSize + NodeLayout.LeafNodeKeySize;
        }

        public (int key, int valueOffset) GetCell(int cellNumber)
        {
            var key = BitConverter.ToInt32(Page.Data, GetCellKeyOffset(cellNumber));
            var valueOffset = GetCellValueOffset(cellNumber);

            return (key, valueOffset);
        }

        public void InsertCell(int cellNumber, int key, Row value)
        {
            if (cellNumber < CellCount)
            {
                throw new NotImplementedException("Move cells after the cursor");
            }

            CellCount += 1;

            BitConverter.GetBytes(key).CopyTo(Page.Data, GetCellKeyOffset(cellNumber));
            value.Serialize(Page.Data, GetCellValueOffset(cellNumber));
        }
    }

    public abstract class Node
    {
        protected readonly Page Page;

        protected Node(Page page)
        {
            Page = page;
        }

        public NodeType Type
        {
            get => (NodeType)Page.Data[NodeLayout.NodeTypeOffset];
            protected set => Page.Data[NodeLayout.NodeTypeOffset] = (byte)value;
        }
    }
}
