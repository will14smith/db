using System;
using SimpleDatabase.Storage.Serialization;
using SimpleDatabase.Storage.Tree;

namespace SimpleDatabase.Execution.Trees
{
    public static class NodeExtensions
    {
        public static IndexKey GetMaxKey(this Node node)
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