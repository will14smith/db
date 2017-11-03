using System;
using SimpleDatabase.Core.Paging;

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
            get => BitConverter.ToInt32(Page.Data, NodeLayout.InternalNodeKeyCountOffset);
            set => BitConverter.GetBytes(value).CopyTo(Page.Data, NodeLayout.InternalNodeKeyCountOffset);
        }

        public int RightChild
        {
            get => BitConverter.ToInt32(Page.Data, NodeLayout.InternalNodeRightChildOffset);
            set => BitConverter.GetBytes(value).CopyTo(Page.Data, NodeLayout.InternalNodeRightChildOffset);
        }

        public int GetCellOffset(int cellNumber)
        {
            return NodeLayout.InternalNodeHeaderSize + cellNumber * NodeLayout.InternalNodeCellSize;
        }
        public int GetChildOffset(int cellNumber)
        {
            return GetCellOffset(cellNumber);
        }
        public int GetKeyOffset(int cellNumber)
        {
            return GetCellOffset(cellNumber) + NodeLayout.InternalNodeChildSize;
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

            return BitConverter.ToInt32(Page.Data, GetChildOffset(childNumber));
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
                BitConverter.GetBytes(child).CopyTo(Page.Data, GetChildOffset(childNumber));
            }
        }

        public int GetKey(int keyNumber)
        {
            return BitConverter.ToInt32(Page.Data, GetKeyOffset(keyNumber));
        }
        public void SetKey(int keyNumber, int key)
        {
            BitConverter.GetBytes(key).CopyTo(Page.Data, GetKeyOffset(keyNumber));
        }

        public void SetCell(int cellNumber, int child, int key)
        {
            SetChild(cellNumber, child);
            SetKey(cellNumber, key);
        }
    }
}