using System.Linq;
using SimpleDatabase.Execution;
using SimpleDatabase.Execution.Operations;
using SimpleDatabase.Execution.Operations.Constants;
using SimpleDatabase.Execution.Operations.Cursors;
using SimpleDatabase.Execution.Operations.Jumps;
using SimpleDatabase.Parsing.Expressions;
using SimpleDatabase.Planning.Items;
using SimpleDatabase.Schemas;

namespace SimpleDatabase.Planning.Iterators;

public class RowIdLookupIterator : IIterator
{
    private readonly IOperationGenerator _generator;
    private readonly Table _table;
    private readonly Expression _rowId;
    
    private readonly SlotItem _cursor;
    private readonly SlotItem _once;

    public RowIdLookupIterator(IOperationGenerator generator, Table table, Expression rowId)
    {
        _generator = generator;
        _table = table;
        _rowId = rowId;

        _cursor = new SlotItem(generator.NewSlot(new SlotDefinition("cursor")));
        _once = new SlotItem(generator.NewSlot(new SlotDefinition("once")));

        Output = new IteratorOutput.Row(_cursor, table.Columns.Select((x, i) => new IteratorOutput.Named(new IteratorOutputName.Constant(x.Name), new ColumnItem(_cursor, i))).ToList());
    }

    public IteratorOutput Output { get; }
    
    public void GenerateInit()
    {
        _generator.Emit(new ConstIntOperation(0));
        _once.Store(_generator);
        
        _generator.Emit(new OpenReadTableOperation(_table));
        
        _generator.EmitNotImplemented("push rowid");
        
        _generator.Emit(new SeekRowIdOperation());
        _cursor.Store(_generator);
    }

    public void GenerateMoveNext(ProgramLabel loopStart, ProgramLabel loopEnd)
    {
        _generator.Emit(new ConstIntOperation(1));
        _once.Load(_generator);
        _generator.Emit(new ConditionalJumpOperation(Comparison.Equal, loopEnd));
        
        _generator.Emit(new ConstIntOperation(1));
        _once.Store(_generator);

        _cursor.Load(_generator);
        _generator.Emit(new NextOperation(loopEnd));
        _cursor.Store(_generator);
    }
}