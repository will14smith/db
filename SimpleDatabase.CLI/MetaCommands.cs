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

            var page = pager.Get(rootPageNumber);
            PrintLeafNode(output, LeafNode.Read(page));
        }

        private static void PrintLeafNode(IREPLOutput output, LeafNode node)
        {
            var numCells = node.CellCount;
            output.WriteLine("leaf (size {0})", numCells);
            for (var i = 0; i < numCells; i++)
            {
                var (key, _) = node.GetCell(i);
                output.WriteLine("  - {0} : {1}", i, key);
            }
        }
    }
}