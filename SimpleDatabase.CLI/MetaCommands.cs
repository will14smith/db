using System;
using SimpleDatabase.Core;
using SimpleDatabase.Core.Paging;
using SimpleDatabase.Core.Trees;

namespace SimpleDatabase.CLI
{
    public class MetaCommands
    {
        public static void PrintConstants(IREPLOutput output)
        {
            output.WriteLine("Constants:");

            output.WriteLine("RowSize: {0}", Row.RowSize);
            output.WriteLine("CommonNodeHeaderSize: {0}", NodeLayout.CommonNodeHeaderSize);
            output.WriteLine("LeafNodeHeaderSize: {0}", NodeLayout.LeafNodeHeaderSize);
            output.WriteLine("LeafNodeCellSize: {0}", NodeLayout.LeafNodeCellSize);
            output.WriteLine("LeafNodeSpaceForCells: {0}", NodeLayout.LeafNodeSpaceForCells);
            output.WriteLine("LeafNodeMaxCells: {0}", NodeLayout.LeafNodeMaxCells);
        }

        public static void PrintBTree(IREPLOutput output, Pager pager, int rootPageNumber)
        {
            output.WriteLine("Tree:");

            PrintNode(output, pager, rootPageNumber, 0);
        }

        private static void PrintNode(IREPLOutput output, Pager pager, int pageNumber, int level)
        {
            var page = pager.Get(pageNumber);
            var node = Node.Read(page);

            switch (node)
            {
                case LeafNode leafNode:
                    output.Indent(level);
                    output.WriteLine("- leaf (size {0})", leafNode.CellCount);

                    for (var i = 0; i < leafNode.CellCount; i++)
                    {
                        output.Indent(level + 1);
                        output.WriteLine("- {0}", leafNode.GetCellKey(i));
                    }
                    break;
                case InternalNode internalNode:
                    output.Indent(level);
                    output.WriteLine("- internal (size {0})", internalNode.KeyCount);

                    for (var i = 0; i < internalNode.KeyCount; i++)
                    {
                        PrintNode(output, pager, internalNode.GetChild(i), level + 1);

                        output.Indent(level);
                        output.WriteLine("- key {0}", internalNode.GetKey(i));
                    }

                    PrintNode(output, pager, internalNode.RightChild, level + 1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
    }
}