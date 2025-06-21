using System;
using System.Collections.Generic;
using System.Linq;
using SimpleDatabase.Planning.Optimisation.Algebra;
using SimpleDatabase.Planning.Optimisation.Rules;
using SimpleDatabase.Planning.Optimisation.Tasks;

namespace SimpleDatabase.Planning.Optimisation;

public class Optimiser
{
    private readonly Stack<IOptimiserTask> _tasks = new();
    private readonly List<Group> _groups = new();
    
    public Plan Optimise(Node rootNode)
    {
        var rootGroup = PopulateGroups(rootNode);
        
        var context = new Context(this);
        context.PushTask(new OptimiseGroupTask(rootGroup));

        while (_tasks.Count > 0)
        {
            var task = _tasks.Pop();
            task.Perform(context);
        }

        var plan = rootGroup.Nodes.OfType<PhysicalNode>().Single();
        
        throw new NotImplementedException();
    }

    private Group PopulateGroups(Node node)
    {
        var group = new Group(_groups.Count);
        _groups.Add(group);

        group.Add(node);

        switch (node)
        {
            case LogicalNode.Get: break;
            case LogicalNode.Filter filter: PopulateGroups(filter.Inner); break;
            case LogicalNode.Projection projection: PopulateGroups(projection.Inner); break;
            case LogicalNode.Join join:
                PopulateGroups(join.Left);
                PopulateGroups(join.Right);
                break;
            
            default: throw new ArgumentOutOfRangeException(nameof(node));
        }
        
        return group;
    }

    public class Context
    {
        public Optimiser Optimiser { get; }
        public IReadOnlyList<IOptimiserRule> Rules { get; } = new IOptimiserRule[]
        {
            // transformation
            new FilterGetFusion(),
            
            // implementation
            new GetToTableScanRule(),
        };
        public IReadOnlyList<Group> Groups => Optimiser._groups;

        public Context(Optimiser optimiser) => Optimiser = optimiser;

        public void PushTask(IOptimiserTask task) => Optimiser._tasks.Push(task);
    }
}