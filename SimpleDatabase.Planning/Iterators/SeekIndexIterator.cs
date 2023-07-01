using System;
using System.Collections.Generic;
using SimpleDatabase.Execution;
using SimpleDatabase.Execution.Operations.Columns;
using SimpleDatabase.Execution.Operations.Constants;
using SimpleDatabase.Execution.Operations.Cursors;
using SimpleDatabase.Execution.Operations.Jumps;
using SimpleDatabase.Parsing.Expressions;
using SimpleDatabase.Planning.Items;
using SimpleDatabase.Schemas;

namespace SimpleDatabase.Planning.Iterators;

public class SeekIndexIterator : IIterator
{
    private readonly IOperationGenerator _generator;
    private readonly Table _table;
    private readonly TableIndex _index;
    private readonly Expression _predicate;
    private readonly bool _writable;

    private readonly SlotItem _cursor;

    public SeekIndexIterator(IOperationGenerator generator, Table table, TableIndex index, Expression predicate, bool writable)
    {
        _generator = generator;
        _table = table;
        _index = index;
        _predicate = predicate;
        _writable = writable;

        _cursor = new SlotItem(_generator.NewSlot(new SlotDefinition($"cursor_index_seek_{index.Name}")));

        Output = ComputeOutput();
    }

    public IteratorOutput Output { get; }

    public void GenerateInit()
    {
        if (_writable)
        {
            throw new NotImplementedException();
        }
        
        if (_predicate is not BinaryExpression { Operator: BinaryOperator.Equal, Left: ColumnNameExpression equalityColumn, Right: LiteralExpression equalityValue })
        {
            throw new NotImplementedException("TODO handle more complex seeks");
        }
        
        if (_index.Structure.Keys[0].Item1.Name != equalityColumn.Name)
        {
            throw new NotImplementedException("assumption: equalityColumn is the left column of the index");
        }
        
        _generator.Emit(equalityValue switch
        {
            NumberLiteralExpression value => new ConstIntOperation(value.Value),
            StringLiteralExpression value => new ConstStringOperation(value.Value),
            
            _ => throw new NotImplementedException("TODO handle more complex seeks"),
        });
        
        _generator.Emit(new OpenReadIndexOperation(_table, _index));
        _generator.Emit(new SeekEqualOperation());
        _cursor.Store(_generator);
    }
    
    public void GenerateMoveNext(ProgramLabel loopStart, ProgramLabel loopEnd)
    {
        _cursor.Load(_generator);
        _generator.Emit(new NextOperation(loopEnd));
        _cursor.Store(_generator);

        CheckEnd(loopEnd);
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }

    private void CheckEnd(ProgramLabel loopEnd)
    {
        if (_predicate is not BinaryExpression { Operator: BinaryOperator.Equal, Left: ColumnNameExpression equalityColumn, Right: LiteralExpression equalityValue })
        {
            throw new NotImplementedException("TODO handle more complex seeks");
        }
        
        if (_index.Structure.Keys[0].Item1.Name != equalityColumn.Name)
        {
            throw new NotImplementedException("assumption: equalityColumn is the left column of the index");
        }

        _cursor.Load(_generator);
        _generator.Emit(new ColumnOperation(1));
        
        _generator.Emit(equalityValue switch
        {
            NumberLiteralExpression value => new ConstIntOperation(value.Value),
            StringLiteralExpression value => new ConstStringOperation(value.Value),
            
            _ => throw new NotImplementedException("TODO handle more complex seeks"),
        });

        _generator.Emit(new ConditionalJumpOperation(Comparison.NotEqual, loopEnd));
    }

    private IteratorOutput ComputeOutput()
    {
        var columns = new List<IteratorOutput.Named>();
        
        var columnIndex = 0;
        {
            var name = new IteratorOutputName.TableColumn(_table.Name, "__rowid");
            var value = new ColumnItem(_cursor, columnIndex++);

            columns.Add(new IteratorOutput.Named(name, value));
        }
        
        foreach (var column in _index.Structure.Keys)
        {
            var name = new IteratorOutputName.TableColumn(_table.Name, column.Item1.Name);
            var value = new ColumnItem(_cursor, columnIndex++);

            columns.Add(new IteratorOutput.Named(name, value));
        }
        
        foreach (var column in _index.Structure.Data)
        {
            var name = new IteratorOutputName.TableColumn(_table.Name, column.Name);
            var value = new ColumnItem(_cursor, columnIndex++);

            columns.Add(new IteratorOutput.Named(name, value));
        }

        return new IteratorOutput.Row(
            _cursor,
            columns
        );
    }
}