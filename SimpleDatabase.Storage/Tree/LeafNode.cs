using System;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;

namespace SimpleDatabase.Storage.Tree
{
    public class LeafNode : Node
    {
        public LeafNode(Page page, IRowSerializer keySerializer, IRowSerializer dataSerializer) 
            : base(page, keySerializer, dataSerializer)
        {
        }

        public static LeafNode New(Page page, IRowSerializer keySerializer, IRowSerializer dataSerializer)
        {
            return new LeafNode(page, keySerializer, dataSerializer)
            {
                Type = PageType.Leaf,
                IsRoot = false,
                CellCount = 0,
                NextLeaf = 0,
            };
        }

        public new static LeafNode Read(Page page, IRowSerializer keySerializer, IRowSerializer dataSerializer)
        {
            if (page.Type != PageType.Leaf)
            {
                throw new InvalidOperationException($"Tried to read a {PageType.Leaf} node but found a {page.Type} node instead");
            }

            return new LeafNode(page, keySerializer, dataSerializer);
        }

        public int CellCount
        {
            get => BitConverter.ToInt32(Page.Data, Layout.LeafNodeCellCountOffset);
            set => BitConverter.GetBytes(value).CopyTo(Page.Data, Layout.LeafNodeCellCountOffset);
        }
        public int NextLeaf
        {
            get => BitConverter.ToInt32(Page.Data, Layout.LeafNodeNextLeafOffset);
            set => BitConverter.GetBytes(value).CopyTo(Page.Data, Layout.LeafNodeNextLeafOffset);
        }

        public Span<byte> GetCellOffset(int index)
        {
            return Page[Layout.LeafNodeHeaderSize + index * Layout.LeafNodeCellSize].Slice(0, Layout.LeafNodeCellSize);
        }
        public Span<byte> GetCellKeyOffset(int index)
        {
            return GetCellOffset(index).Slice(0, Layout.LeafNodeKeySize);
        }
        public Span<byte> GetCellValueOffset(int index)
        {
            return GetCellOffset(index).Slice(Layout.LeafNodeKeySize, Layout.LeafNodeValueSize);
        }

        public Row GetCellKey(int cellNumber)
        {
            return KeySerializer.ReadRow(GetCellKeyOffset(cellNumber));
        }
        public Row GetCellValue(int cellNumber)
        {
            return DataSerializer.ReadRow(GetCellValueOffset(cellNumber));
        }

        public void SetCell(int cellNumber, Row key, Row data)
        {
            KeySerializer.WriteRow(GetCellKeyOffset(cellNumber), key);
            DataSerializer.WriteRow(GetCellValueOffset(cellNumber), data);
        }

        public void CopyCell(LeafNode source, int sourceCell, int destinationCell)
        {
            var src = source.GetCellOffset(sourceCell);
            var dst = GetCellOffset(destinationCell);

            src.CopyTo(dst);
        }
    }
}
