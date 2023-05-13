using System;
using SimpleDatabase.Execution;
using SimpleDatabase.Execution.Operations;
using SimpleDatabase.Execution.Operations.Constants;
using SimpleDatabase.Execution.Operations.Jumps;
using SimpleDatabase.Planning.Nodes;

namespace SimpleDatabase.Planning.Iterators;

public class ExplainIterator : IIterator
{
    private readonly IOperationGenerator _generator;
    private readonly Node _plan;
    private readonly Program _program;

    public ExplainIterator(IOperationGenerator generator, Node plan, Program program)
    {
        _generator = generator;
        _plan = plan;
        _program = program;
    }

    public IteratorOutput Output { get; } = new IteratorOutput.Void();
    
    public void GenerateInit() { }

    public void GenerateMoveNext(ProgramLabel loopStart, ProgramLabel loopEnd)
    {
        Yield("=== Plan ===");
        YieldNode(_plan, string.Empty);
        Yield(string.Empty);

        Yield("=== Program ===");
        foreach (var line in _program.ToString().Split(Environment.NewLine))
        {
            Yield(line);
        }
        
        _generator.Emit(new JumpOperation(loopEnd));
    }
    
    private void YieldNode(Node node, string indent)
    {
        switch (node)
        {
            case ConstantNode constantNode:
                Yield($"{indent}constant ({string.Join(", ", constantNode.Columns)}) ({constantNode.Values.Count} rows)");
                break;
            
            case ProjectionNode projectionNode:
                Yield($"{indent}projection ({string.Join(", ", projectionNode.Columns)})");
                YieldNode(projectionNode.Input, $"{indent}  ");
                break;
            
            case FilterNode filterNode:
                Yield($"{indent}filter ({filterNode.Predicate})");
                YieldNode(filterNode.Input, $"{indent}  ");
                break;
            
            case ScanTableNode scanNode:
                Yield($"{indent}scan ({scanNode.TableName})");
                break;

            default: 
                Yield($"{indent}unsupported plan node: {node.GetType().FullName}");
                break;
        }
    }
    
    private void Yield(string text)
    {
        _generator.Emit(new ConstStringOperation(text));
        _generator.Emit(new MakeRowOperation(1));
        _generator.Emit(new YieldOperation());
    }
}