using System;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;

namespace SimpleDatabase.Storage.Tree
{
    public abstract class Node
    {
        protected readonly Page Page;

        protected readonly IRowSerializer KeySerializer;
        protected readonly IRowSerializer DataSerializer;

        public NodeLayout Layout { get; }

        protected Node(Page page, IRowSerializer keySerializer, IRowSerializer dataSerializer)
        {
            Page = page;

            KeySerializer = keySerializer;
            DataSerializer = dataSerializer;

            Layout = new NodeLayout(keySerializer, dataSerializer);
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
        
        public static Node Read(Page page, IRowSerializer keySerializer, IRowSerializer dataSerializer)
        {
            switch (page.Type)
            {
                case PageType.Internal:
                    return InternalNode.Read(page, keySerializer, dataSerializer);
                case PageType.Leaf:
                    return LeafNode.Read(page, keySerializer, dataSerializer);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}