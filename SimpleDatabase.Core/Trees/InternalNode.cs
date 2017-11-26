using System;
using System.Text;
using SimpleDatabase.Core.Paging;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Core.Trees
{
    public class InternalNode : Node
    {
        private InternalNode(Page page) : base(page)
        {
        }

        public static InternalNode New(Page page)
        {
            return new InternalNode(page)
            {
                Type = NodeType.Internal,
                IsRoot = false,
                KeyCount = 0
            };
        }

        public new static InternalNode Read(Page page)
        {
            var type = GetType(page);
            if (type != NodeType.Internal)
            {
                throw new InvalidOperationException($"Tried to read a {NodeType.Internal} node but found a {type} node instead");
            }

            return new InternalNode(page);
        }

        public int KeyCount
        {
            get => Page[NodeLayout.InternalNodeKeyCountOffset].Read<int>();
            set => Page[NodeLayout.InternalNodeKeyCountOffset].Write(value);
        }

        public int RightChild
        {
            get => Page[NodeLayout.InternalNodeRightChildOffset].Read<int>();
            set => Page[NodeLayout.InternalNodeRightChildOffset].Write(value);

        }

        public Span<byte> GetCellOffset(int cellNumber)
        {
            return Page[NodeLayout.InternalNodeHeaderSize + cellNumber * NodeLayout.InternalNodeCellSize].Slice(0, NodeLayout.InternalNodeCellSize);
        }
        public Span<byte> GetChildOffset(int cellNumber)
        {
            return GetCellOffset(cellNumber);
        }
        public Span<byte> GetKeyOffset(int cellNumber)
        {
            return GetCellOffset(cellNumber).Slice(NodeLayout.InternalNodeChildSize);
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

        public int GetKey(int keyNumber)
        {
            return GetKeyOffset(keyNumber).Read<int>();
        }
        public void SetKey(int keyNumber, int key)
        {
            GetKeyOffset(keyNumber).Write(key);
        }

        public void SetCell(int cellNumber, int child, int key)
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