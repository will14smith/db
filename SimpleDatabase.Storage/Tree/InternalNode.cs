using System;
using System.Text;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Storage.Tree
{
    public class InternalNode : Node
    {
        private InternalNode(Page page, IRowSerializer keySerializer, IRowSerializer dataSerializer) 
            : base(page, keySerializer, dataSerializer)
        {
        }

        public static InternalNode New(Page page, IRowSerializer keySerializer, IRowSerializer dataSerializer)
        {
            return new InternalNode(page, keySerializer, dataSerializer)
            {
                Type = PageType.Internal,
                IsRoot = false,
                KeyCount = 0
            };
        }

        public new static InternalNode Read(Page page, IRowSerializer keySerializer, IRowSerializer dataSerializer)
        {
            if (page.Type != PageType.Internal)
            {
                throw new InvalidOperationException($"Tried to read a {PageType.Internal} node but found a {page.Type} node instead");
            }

            return new InternalNode(page, keySerializer, dataSerializer);
        }

        public int KeyCount
        {
            get => Page[Layout.InternalNodeKeyCountOffset].Read<int>();
            set => Page[Layout.InternalNodeKeyCountOffset].Write(value);
        }

        public int RightChild
        {
            get => Page[Layout.InternalNodeRightChildOffset].Read<int>();
            set => Page[Layout.InternalNodeRightChildOffset].Write(value);

        }

        public Span<byte> GetCellOffset(int cellNumber)
        {
            return Page[Layout.InternalNodeHeaderSize + cellNumber * Layout.InternalNodeCellSize].Slice(0, Layout.InternalNodeCellSize);
        }
        public Span<byte> GetChildOffset(int cellNumber)
        {
            return GetCellOffset(cellNumber);
        }
        public Span<byte> GetKeyOffset(int cellNumber)
        {
            return GetCellOffset(cellNumber).Slice(Layout.InternalNodeChildSize, Layout.InternalNodeKeySize);
        }

        public int GetChild(int childNumber)
        {
            if (childNumber > KeyCount)
            {
                throw new InvalidOperationException($"Tried to access childNumber { childNumber } > KeyCount { KeyCount}");
            }

            if (childNumber == KeyCount)
            {
                return RightChild;
            }

            return GetChildOffset(childNumber).Read<int>();
        }
        public void SetChild(int childNumber, int child)
        {
            if (childNumber > KeyCount)
            {
                throw new InvalidOperationException($"Tried to access childNumber { childNumber } > KeyCount { KeyCount}");
            }

            if (childNumber == KeyCount)
            {
                RightChild = child;
            }
            else
            {
                GetChildOffset(childNumber).Write(child);
            }
        }

        public Row GetKey(int keyNumber)
        {
            return KeySerializer.ReadRow(GetKeyOffset(keyNumber));
        }
        public void SetKey(int keyNumber, Row key)
        {
            KeySerializer.WriteRow(GetKeyOffset(keyNumber), key);
        }

        public void SetCell(int cellNumber, int child, Row key)
        {
            SetChild(cellNumber, child);
            SetKey(cellNumber, key);
        }

        public void CopyCell(InternalNode source, int sourceCell, int destinationCell)
        {
            var src = source.GetCellOffset(sourceCell);
            var dst = GetCellOffset(destinationCell);

            src.CopyTo(dst);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            for (var i = 0; i < KeyCount; i++)
            {
                sb.Append($"p{GetChild(i)}, ");
                sb.Append($"k{GetKey(i)}, ");
            }

            sb.Append($"p{GetChild(KeyCount)}");

            return sb.ToString();
        }
    }
}