using System;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;

namespace SimpleDatabase.Storage.Tree
{
    public abstract class Node
    {
        protected readonly Page Page;
        protected readonly IIndexSerializer Serializer;

        public NodeLayout Layout { get; }

        protected Node(Page page, IIndexSerializer serializer)
        {
            Page = page;
            Serializer = serializer;

            Layout = new NodeLayout(Serializer);
        }

        public PageId PageId => Page.Id;

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
        
        public static Node Read(Page page, IIndexSerializer serializer)
        {
            switch (page.Type)
            {
                case PageType.Internal:
                    return InternalNode.Read(page, serializer);
                case PageType.Leaf:
                    return LeafNode.Read(page, serializer);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}