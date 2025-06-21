using System.Collections.Generic;
using System.Linq;
using SimpleDatabase.Planning.Optimisation.Algebra;
using SimpleDatabase.Planning.Optimisation.Rules;

namespace SimpleDatabase.Planning.Optimisation.Tasks;

public class ExploreExpressionTask : IOptimiserTask
{
    private readonly Node _node;

    public ExploreExpressionTask(Node node)
    {
        _node = node;
    }

    public void Perform(Optimiser.Context context)
    {
        var matched = new List<ITransformRule>();
        
        foreach (var rule in context.Rules.OfType<ITransformRule>())
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
            context.PushTask(new ApplyRuleTask(rule, _node, true));

            // TODO task to explore input groups
        }
    }
}