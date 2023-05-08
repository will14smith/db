using System;
using System.Linq;
using SimpleDatabase.Schemas;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Tree;

namespace SimpleDatabase.CLI.Commands;

public class BTreeCommand : ICommand
{
    private readonly REPLState _state;
    private readonly IREPLOutput _output;

    public BTreeCommand(REPLState state, IREPLOutput output)
    {
        _state = state;
        _output = output;
    }
    
    public CommandResponse Handle(string[] args)
    {
        var table = _state.Table;
        
        var index = table.Indices.FirstOrDefault();
        if (index != null)
        {
            PrintBTree(table, index);
        }

        return new CommandResponse.Success();
    }
    
    private void PrintBTree(Table table, TableIndex index)
    {
        _output.WriteLine("Tree:");

        var sourcePager = new SourcePager(_state.Pager, new PageSource.Index(table.Name, index.Name));
        PrintNode(sourcePager, table, index.RootPage, 0);
    }

    private void PrintNode(ISourcePager pager, Table table, int pageNumber, int level)
    {
        var page = pager.Get(pageNumber);
        // TODO
        var node = Node.Read(page, null);

        switch (node)
        {
            case LeafNode leafNode:
                _output.Indent(level);
                _output.WriteLine("- leaf (size {0})", leafNode.CellCount);

                for (var i = 0; i < leafNode.CellCount; i++)
                {
                    _output.Indent(level + 1);
                    _output.WriteLine("- {0}", leafNode.GetCellKey(i));
                }

                break;
            case InternalNode internalNode:
                _output.Indent(level);
                _output.WriteLine("- internal (size {0})", internalNode.KeyCount);

                for (var i = 0; i < internalNode.KeyCount; i++)
                {
                    PrintNode(pager, table, internalNode.GetChild(i), level + 1);

                    _output.Indent(level);
                    _output.WriteLine("- key {0}", internalNode.GetKey(i));
                }

                PrintNode(pager, table, internalNode.RightChild, level + 1);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}