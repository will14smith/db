using System;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Storage.Tree
{
    public class LeafNode : Node
    {
        private LeafNode(IRowSerializer rowSerializer, Page page) : base(rowSerializer, page)
        {
        }

        public static LeafNode New(IRowSerializer rowSerializer, Page page)
        {
            return new LeafNode(rowSerializer, page)
            {
                Type = PageType.Leaf,
                IsRoot = false,
                CellCount = 0,
                NextLeaf = 0,
            };
        }

        public new static LeafNode Read(IRowSerializer rowSerializer, Page page)
        {
            if (page.Type != PageType.Leaf)
            {
                throw new InvalidOperationException($"Tried to read a {PageType.Leaf} node but found a {page.Type} node instead");
            }

            return new LeafNode(rowSerializer, page);
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
            return GetCellOffset(index);
        }
        public Span<byte> GetCellValueOffset(int index)
        {
            return GetCellOffset(index).Slice(Layout.LeafNodeKeySize, Layout.LeafNodeValueSize);
        }

        public int GetCellKey(int cellNumber)
        {
            return GetCellKeyOffset(cellNumber).Read<int>();
        }
        public Row GetCellValue(int cellNumber)
        {
            return RowSerializer.ReadRow(GetCellValueOffset(cellNumber));
        }
        public ColumnValue GetCellColumn(int cellNumber, int columnIndex)
        {
            return RowSerializer.ReadColumn(GetCellValueOffset(cellNumber), columnIndex);
        }

        public void SetCell(int cellNumber, int key, Row row)
        {
            GetCellKeyOffset(cellNumber).Write(key);
            RowSerializer.WriteRow(GetCellValueOffset(cellNumber), row);
        }
        public void SetCellColumn(int cellNumber, int columnIndex, ColumnValue column)
        {
            RowSerializer.WriteColumn(GetCellValueOffset(cellNumber), columnIndex, column);
        }

        public void CopyCell(LeafNode source, int sourceCell, int destinationCell)
        {
            var src = source.GetCellOffset(sourceCell);
            var dst = GetCellOffset(destinationCell);

            src.CopyTo(dst);
        }
    }
}
