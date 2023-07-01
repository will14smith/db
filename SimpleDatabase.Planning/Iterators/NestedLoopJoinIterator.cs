using System;
using System.Collections.Generic;
using SimpleDatabase.Execution;
using SimpleDatabase.Execution.Operations;
using SimpleDatabase.Execution.Operations.Constants;
using SimpleDatabase.Execution.Operations.Jumps;
using SimpleDatabase.Execution.Operations.Slots;
using SimpleDatabase.Parsing.Expressions;
using SimpleDatabase.Planning.Items;

namespace SimpleDatabase.Planning.Iterators;

public class NestedLoopJoinIterator : IIterator
{
    private readonly IOperationGenerator _generator;
    
    private readonly IIterator _outerIterator;
    private readonly IIterator _innerIterator;
    private readonly Expression? _joinPredicate;
    
    private readonly SlotItem _needOuter;

    public NestedLoopJoinIterator(IOperationGenerator generator, IIterator outerIterator, IIterator innerIterator, Expression? joinPredicate)
    {
        _generator = generator;
        _outerIterator = outerIterator;
        _innerIterator = innerIterator;
        _joinPredicate = joinPredicate;

        _needOuter = new SlotItem(_generator.NewSlot(new SlotDefinition("nested_loop_need_outer")));

        Output = GenerateOutput();
    }
    
    public IteratorOutput Output { get; }
    
    public void GenerateInit()
    {
        _generator.Emit(new ConstIntOperation(1));
        _needOuter.Store(_generator);
        
        _outerIterator.GenerateInit();
        _innerIterator.GenerateInit();
    }

    public void GenerateMoveNext(ProgramLabel loopStart, ProgramLabel loopEnd)
    {
        var innerStart = _generator.NewLabel("nested_loop_inner_start");
        var innerEnd = _generator.NewLabel("nested_loop_inner_end");
        var checkPredicate = _generator.NewLabel("nested_loop_predicate");
        
        // if _needOuter:
        //   var outerResult = outer.MoveNext()
        //   if isEndOfIter(outerResult) { goto loopEnd }
        //   inner.Reset()
        //   needOuter = false 
        _generator.Emit(new ConstIntOperation(1));
        _needOuter.Load(_generator);
        _generator.Emit(new ConditionalJumpOperation(Comparison.NotEqual, innerStart));
        
        _outerIterator.GenerateMoveNext(loopStart, loopEnd);
        _generator.Emit(new ConstIntOperation(0));
        _needOuter.Load(_generator);

        _innerIterator.Reset();
        
        _generator.MarkLabel(innerStart);
        
        // do {
        //   var innerResult = inner.MoveNext();
        _innerIterator.GenerateMoveNext(innerStart, innerEnd);
       _generator.Emit(new JumpOperation(checkPredicate));
        
        //   if isEndOfIter(innerResult) { _needOuter = true; goto loopStart }
        _generator.MarkLabel(innerEnd);
        _generator.Emit(new ConstIntOperation(1));
        _needOuter.Store(_generator);
        _generator.Emit(new JumpOperation(loopStart));
        
        //   if pred(outer, inner) { yield; }
        _generator.MarkLabel(checkPredicate);
        if (_joinPredicate != null)
        {
            throw new NotImplementedException();
        }
        // } while(true);
    }

    public void Reset()
    {
        throw new System.NotImplementedException();
    }

    private IteratorOutput GenerateOutput()
    {
        var columns = new List<IteratorOutput.Named>();

        columns.AddRange(((IteratorOutput.Row)_outerIterator.Output).Columns);
        columns.AddRange(((IteratorOutput.Row)_innerIterator.Output).Columns);
        
        return new JoinOutput(columns);
    }

    public class JoinOutput : IteratorOutput.Row
    {
        public JoinOutput(IReadOnlyList<Named> columns) : base(null, columns)
        {
        }
    }
}