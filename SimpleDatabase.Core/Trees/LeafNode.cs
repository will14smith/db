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
            return new LeafNode(page)
            {
                Type = NodeType.Leaf,
                IsRoot = false,
                CellCount = 0
                NextLeaf = 0,
            };
        }

        public new static LeafNode Read(Page page)
        {
            var type = GetType(page);
            if (type != NodeType.Leaf)
            {
                throw new InvalidOperationException($"Tried to read a {NodeType.Leaf} node but found a {type} node instead");
            }

            return new LeafNode(page);
        }

        public int CellCount
        {
            get => BitConverter.ToInt32(Page.Data, NodeLayout.LeafNodeCellCountOffset);
            set => BitConverter.GetBytes(value).CopyTo(Page.Data, NodeLayout.LeafNodeCellCountOffset);
        }
        public int NextLeaf
        {
            get => BitConverter.ToInt32(Page.Data, NodeLayout.LeafNodeNextLeafOffset);
            set => BitConverter.GetBytes(value).CopyTo(Page.Data, NodeLayout.LeafNodeNextLeafOffset);
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
                    CopyCell(this, i - 1, i);
                }
            }

            CellCount += 1;

            SetCell(cellNumber, key, value);
        }

        public void SetCell(int cellNumber, int key, Row value)
        {
            BitConverter.GetBytes(key).CopyTo(Page.Data, GetCellKeyOffset(cellNumber));
            value.Serialize(Page.Data, GetCellValueOffset(cellNumber));
        }

        public void CopyCell(LeafNode source, int sourceCell, int destinationCell)
        {
            Array.Copy(
                source.Page.Data, GetCellOffset(sourceCell),
                Page.Data, GetCellOffset(destinationCell),
                NodeLayout.LeafNodeCellSize);
        }
    }
}
