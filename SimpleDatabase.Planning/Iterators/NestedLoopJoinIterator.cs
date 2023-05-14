using System.Collections.Generic;
using SimpleDatabase.Execution;
using SimpleDatabase.Execution.Operations;
using SimpleDatabase.Parsing.Expressions;
using SimpleDatabase.Planning.Items;

namespace SimpleDatabase.Planning.Iterators;

public class NestedLoopJoinIterator : IIterator
{
    private readonly IOperationGenerator _generator;
    
    private readonly IIterator _outerIterator;
    private readonly IIterator _innerIterator;
    private readonly Expression? _joinPredicate;

    public NestedLoopJoinIterator(IOperationGenerator generator, IIterator outerIterator, IIterator innerIterator, Expression? joinPredicate)
    {
        _generator = generator;
        _outerIterator = outerIterator;
        _innerIterator = innerIterator;
        _joinPredicate = joinPredicate;

        Output = GenerateOutput();
    }
    
    public IteratorOutput Output { get; }
    
    public void GenerateInit()
    {
        _outerIterator.GenerateInit();
    }

    public void GenerateMoveNext(ProgramLabel loopStart, ProgramLabel loopEnd)
    {
        // TODO only moving to next outer when inner is finished
        _outerIterator.GenerateMoveNext(loopStart, loopEnd);
        
        // TODO only init inner when moving to next outer
        _innerIterator.GenerateInit();
        // TODO use correct labels
        _innerIterator.GenerateMoveNext(loopStart, loopEnd);
        
        // TODO check the predicate
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