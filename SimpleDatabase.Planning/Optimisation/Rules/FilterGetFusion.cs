using System.Collections.Generic;
using SimpleDatabase.Planning.Optimisation.Algebra;

namespace SimpleDatabase.Planning.Optimisation.Rules;

public class FilterGetFusion : ITransformRule
{
    public bool IsMatch(Node node) => node is LogicalNode.Filter { Inner: LogicalNode.Get };

    public IEnumerable<Node> Apply(Node node)
    {
        var filter = (LogicalNode.Filter)node;
        var get = (LogicalNode.Get)filter.Inner;

        var fusedPredicate = get.Predicate == null ? filter.Predicate : new AlgebraExpression.And(get.Predicate, filter.Predicate);
        
        yield return get with { Predicate = fusedPredicate };
    }
}