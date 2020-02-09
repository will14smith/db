using System;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;

namespace SimpleDatabase.Storage.Tree
{
    public class LeafNode : Node
    {
        public LeafNode(Page page, IIndexSerializer? serializer) 
            : base(page, serializer)
        {
        }

        public static LeafNode New(Page page, IIndexSerializer? serializer)
        {
            return new LeafNode(page, serializer)
            {
                Type = PageType.Leaf,
                IsRoot = false,
                CellCount = 0,
                NextLeaf = 0,
            };
        }

        public new static LeafNode Read(Page page, IIndexSerializer? serializer)
        {
            if (page.Type != PageType.Leaf)
            {
                throw new InvalidOperationException($"Tried to read a {PageType.Leaf} node but found a {page.Type} node instead");
            }

            return new LeafNode(page, serializer);
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

        public IndexKey GetCellKey(int cellNumber)
        {
            return Serializer.ReadKey(GetCellKeyOffset(cellNumber));
        }
        public IndexData GetCellValue(int cellNumber)
        {
            return Serializer.ReadData(GetCellValueOffset(cellNumber));
        }

        public void SetCell(int cellNumber, IndexKey key, IndexData data)
        {
            Serializer.WriteKey(GetCellKeyOffset(cellNumber), key);
            Serializer.WriteData(GetCellValueOffset(cellNumber), data);
        }

        public void CopyCell(LeafNode source, int sourceCell, int destinationCell)
        {
            var src = source.GetCellOffset(sourceCell);
            var dst = GetCellOffset(destinationCell);

            src.CopyTo(dst);
        }
    }
}
