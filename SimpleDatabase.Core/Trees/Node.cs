using System;
using SimpleDatabase.Core.Paging;

namespace SimpleDatabase.Core.Trees
{
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

        public bool IsRoot
        {
            get => Page.Data[NodeLayout.IsRootOffset] == 1;
            set => Page.Data[NodeLayout.IsRootOffset] = (byte)(value ? 1 : 0);
        }

        public static NodeType GetType(Page page)
        {
            return (NodeType)page.Data[NodeLayout.NodeTypeOffset];
        }

        public static Node Read(Page page)
        {
            switch (GetType(page))
            {
                case NodeType.Internal:
                    return InternalNode.Read(page);
                case NodeType.Leaf:
                    return LeafNode.Read(page);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}