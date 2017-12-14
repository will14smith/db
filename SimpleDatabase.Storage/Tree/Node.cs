using System;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;

namespace SimpleDatabase.Storage.Tree
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

        public PageType Type
        {
            get => Page.Type;
            protected set => Page.Type = value;
        }

        public bool IsRoot
        {
            get => Page.Data[Layout.IsRootOffset] == 1;
            set => Page.Data[Layout.IsRootOffset] = (byte)(value ? 1 : 0);
        }
        
        public static Node Read(IRowSerializer rowSerializer, Page page)
        {
            switch (page.Type)
            {
                case PageType.Internal:
                    return InternalNode.Read(rowSerializer, page);
                case PageType.Leaf:
                    return LeafNode.Read(rowSerializer, page);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}