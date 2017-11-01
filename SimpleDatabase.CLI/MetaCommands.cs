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
            throw new System.NotImplementedException();
        }
    }
}