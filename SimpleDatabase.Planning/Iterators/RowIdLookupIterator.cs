using System;
using System.Linq;
using SimpleDatabase.Execution;
using SimpleDatabase.Execution.Operations.Constants;
using SimpleDatabase.Execution.Operations.Cursors;
using SimpleDatabase.Execution.Operations.Jumps;
using SimpleDatabase.Parsing.Expressions;
using SimpleDatabase.Planning.Items;
using SimpleDatabase.Planning.Nodes;
using SimpleDatabase.Schemas;

namespace SimpleDatabase.Planning.Iterators;

public class RowIdLookupIterator : IIterator
{
    private readonly IOperationGenerator _generator;
    private readonly Scope _scope;
    private readonly Table _table;
    private readonly Expression _rowId;
    
    private readonly SlotItem _cursor;
    private readonly SlotItem _once;

    public RowIdLookupIterator(IOperationGenerator generator, Scope scope, Table table, Expression rowId)
    {
        _generator = generator;
        _scope = scope;
        _table = table;
        _rowId = rowId;

        _cursor = new SlotItem(_generator.NewSlot(new SlotDefinition($"cursor_row_id_{table.Name}")));
        _once = new SlotItem(_generator.NewSlot(new SlotDefinition($"once_row_id_{table.Name}")));

        Output = new IteratorOutput.Row(_cursor, table.Columns.Select((x, i) => new IteratorOutput.Named(new IteratorOutputName.Constant(x.Name), new ColumnItem(_cursor, i))).ToList());
    }

    public IteratorOutput Output { get; }
    
    public void GenerateInit()
    {
        _generator.Emit(new ConstIntOperation(0));
        _once.Store(_generator);
        
        PushRowId();
        _generator.Emit(new OpenReadTableOperation(_table));
        _generator.Emit(new SeekRowIdOperation());
        _cursor.Store(_generator);
    }

    private void PushRowId()
    {
        if (_rowId is not NodeOutputExpression nodeOutput)
        {
            throw new NotImplementedException("support other rowid sources");
        }

        var output = _scope.Get(new ScopeVariable.Indexed(new ScopeVariable.Named(nodeOutput.NodeAlias), nodeOutput.Index));
        output.Load(_generator);
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