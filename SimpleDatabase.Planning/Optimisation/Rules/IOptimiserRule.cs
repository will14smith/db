using System.Collections.Generic;
using SimpleDatabase.Planning.Optimisation.Algebra;

namespace SimpleDatabase.Planning.Optimisation.Rules;

public interface IOptimiserRule
{
    bool IsMatch(Node node);
    IEnumerable<Node> Apply(Node node);
}

public interface ITransformRule : IOptimiserRule { }
public interface IImplementationRule : IOptimiserRule { }
