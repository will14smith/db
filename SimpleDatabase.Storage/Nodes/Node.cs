using System;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;

namespace SimpleDatabase.Storage.Nodes
{
    public abstract class Node
    {
        protected readonly IRowSerializer RowSerializer;
        protected readonly Page Page;

        public NodeLayout Layout { get; }

        protected Node(IRowSerializer rowSerializer, Page page)
        {
            RowSerializer = rowSerializer;
            Page = page;

            Layout = new NodeLayout(rowSerializer);
        }

        public int PageNumber => Page.Number;

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

        public static Node Read(IRowSerializer rowSerializer, Page page)
        {
            switch (GetType(page))
            {
                case NodeType.Internal:
                    return InternalNode.Read(rowSerializer, page);
                case NodeType.Leaf:
                    return LeafNode.Read(rowSerializer, page);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}