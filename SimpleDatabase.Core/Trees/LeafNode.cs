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

        public int GetCellOffset(int index)
        {
            return NodeLayout.LeafNodeHeaderSize + index * NodeLayout.LeafNodeCellSize;
        }
        public int GetCellKeyOffset(int index)
        {
            return GetCellOffset(index);
        }
        public int GetCellValueOffset(int index)
        {
            return GetCellOffset(index) + NodeLayout.LeafNodeKeySize;
        }

        public int GetCellKey(int cellNumber)
        {
            return BitConverter.ToInt32(Page.Data, GetCellKeyOffset(cellNumber));
        }

        public void InsertCell(int cellNumber, int key, Row value)
        {
            if (cellNumber < CellCount)
            {
                for (var i = CellCount; i > cellNumber; i--)
                {
                    Array.Copy(
                        Page.Data, GetCellOffset(i - 1),
                        Page.Data, GetCellOffset(i),
                        NodeLayout.LeafNodeCellSize);
                }
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
            get => GetType(Page);
            protected set => Page.Data[NodeLayout.NodeTypeOffset] = (byte)value;
        }

        public static NodeType GetType(Page page)
        {
            return (NodeType)page.Data[NodeLayout.NodeTypeOffset];
        }
    }
}
