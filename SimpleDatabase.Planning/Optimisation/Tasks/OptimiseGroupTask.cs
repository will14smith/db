using System.Linq;
using SimpleDatabase.Planning.Optimisation.Algebra;

namespace SimpleDatabase.Planning.Optimisation.Tasks;

public class OptimiseGroupTask : IOptimiserTask
{
    private readonly Group _group;

    public OptimiseGroupTask(Group group)
    {
        _group = group;
    }

    public void Perform(Optimiser.Context context)
    {
        // TODO check if we've anything to do
        
        // optimise logical expressions in group
        foreach (var logicalNode in _group.Nodes.OfType<LogicalNode>())
        {
            context.PushTask(new OptimiseExpressionTask(logicalNode));
        }

        // optimise/cost physical expressions in group
        foreach (var physicalNode in _group.Nodes.OfType<PhysicalNode>())
        {
            context.PushTask(new OptimiseInputsTask(physicalNode));
        }
    }
}