using System.Collections.Generic;
using SimpleDatabase.Planning.Optimisation.Algebra;

namespace SimpleDatabase.Planning.Optimisation.Rules;

public class GetToTableScanRule : IImplementationRule
{
    public bool IsMatch(Node node)
    {
        return node is LogicalNode.Get;
    }

    public IEnumerable<Node> Apply(Node node)
    {
        var get = (LogicalNode.Get) node;

        yield return new PhysicalNode.TableScan(get.Table, get.Predicate);
    }
}