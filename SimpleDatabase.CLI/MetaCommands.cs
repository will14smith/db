using System;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;
using SimpleDatabase.Storage.Tree;

namespace SimpleDatabase.CLI
{
    public class MetaCommands
    {
        public static void PrintBTree(IREPLOutput output, Pager pager, StoredTable table)
        {
            output.WriteLine("Tree:");

            PrintNode(output, pager, table.Table, table.RootPageNumber, 0);
        }

        private static void PrintNode(IREPLOutput output, Pager pager, Table table, int pageNumber, int level)
        {
            var page = pager.Get(pageNumber);
            var node = Node.Read(new RowSerializer(table, new ColumnTypeSerializerFactory()), page);

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
                        PrintNode(output, pager, table, internalNode.GetChild(i), level + 1);

                        output.Indent(level);
                        output.WriteLine("- key {0}", internalNode.GetKey(i));
                    }

                    PrintNode(output, pager, table, internalNode.RightChild, level + 1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
    }
}