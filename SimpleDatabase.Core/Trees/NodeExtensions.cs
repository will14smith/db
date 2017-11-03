using System;

namespace SimpleDatabase.Core.Trees
{
    public static class NodeExtensions
    {
        public static int GetMaxKey(this Node node)
        {
            switch (node)
            {
                case InternalNode internalNode:
                    return internalNode.GetKey(internalNode.KeyCount - 1);
                case LeafNode leafNode:
                    return leafNode.GetCellKey(leafNode.CellCount - 1);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}