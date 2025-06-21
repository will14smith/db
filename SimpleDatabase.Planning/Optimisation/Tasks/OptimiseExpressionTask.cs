using System.Collections.Generic;
using SimpleDatabase.Planning.Optimisation.Algebra;
using SimpleDatabase.Planning.Optimisation.Rules;

namespace SimpleDatabase.Planning.Optimisation.Tasks;

public class OptimiseExpressionTask : IOptimiserTask
{
    private readonly Node _node;

    public OptimiseExpressionTask(Node node)
    {
        _node = node;
    }

    public void Perform(Optimiser.Context context)
    {
        var matched = new List<IOptimiserRule>();
        
        foreach (var rule in context.Rules)
        {
            // TODO check if rule has been applied already
            if (rule.IsMatch(_node))
            {
                matched.Add(rule);
            }
        }
        
        // TODO sort rules by priority
        
        foreach (var rule in matched)
        {
            context.PushTask(new ApplyRuleTask(rule, _node, false));

            // TODO task to explore input groups
        }
    }
}