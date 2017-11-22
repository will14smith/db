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
                CellCount = 0,
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

        public Span<byte> GetCellOffset(int index)
        {
            return Page[NodeLayout.LeafNodeHeaderSize + index * NodeLayout.LeafNodeCellSize].Slice(0, NodeLayout.LeafNodeCellSize);
        }
        public Span<byte> GetCellKeyOffset(int index)
        {
            return GetCellOffset(index);
        }
        public Span<byte> GetCellValueOffset(int index)
        {
            return GetCellOffset(index).Slice(NodeLayout.LeafNodeKeySize);
        }

        public int GetCellKey(int cellNumber)
        {
            return GetCellKeyOffset(cellNumber).Read<int>();
        }
        public Row GetCellValue(int cellNumber)
        {
            return Row.Deserialize(GetCellValueOffset(cellNumber));
        }
        public object GetCellColumn(int cellNumber, int columnIndex)
        {
            // TODO optimise this, don't need to read the whole row
            return GetCellValue(cellNumber).GetColumn(columnIndex);
        }

        public void SetCell(int cellNumber, int key, Row value)
        {
            GetCellKeyOffset(cellNumber).Write(key);
            value.Serialize(GetCellValueOffset(cellNumber));
        }

        public void CopyCell(LeafNode source, int sourceCell, int destinationCell)
        {
            var src = source.GetCellOffset(sourceCell);
            var dst = GetCellOffset(destinationCell);

            src.CopyTo(dst);
        }
    }
}
