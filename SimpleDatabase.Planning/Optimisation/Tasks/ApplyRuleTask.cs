using System.Linq;
using SimpleDatabase.Planning.Optimisation.Algebra;
using SimpleDatabase.Planning.Optimisation.Rules;

namespace SimpleDatabase.Planning.Optimisation.Tasks;

public class ApplyRuleTask : IOptimiserTask
{
    private readonly IOptimiserRule _rule;
    private readonly Node _before;
    private readonly bool _explore;

    public ApplyRuleTask(IOptimiserRule rule, Node before, bool explore)
    {
        _rule = rule;
        _before = before;
        _explore = explore;
    }

    public void Perform(Optimiser.Context context)
    {
        // TODO check if rule has been applied

        foreach (var after in _rule.Apply(_before))
        {
            // TODO lookup group by hash
            var group = context.Groups.Single(x => x.Nodes.Contains(_before));
            group.Add(after);
            
            // TODO add new children to existing/new groups

            switch (after)
            {
                case LogicalNode:
                    context.PushTask(_explore ? new ExploreExpressionTask(after) : new OptimiseExpressionTask(after));
                    break;
                case PhysicalNode:
                    context.PushTask(new OptimiseInputsTask(after));
                    break;
            }
        }
    }
}