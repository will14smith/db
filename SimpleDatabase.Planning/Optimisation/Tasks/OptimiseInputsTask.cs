using SimpleDatabase.Planning.Optimisation.Algebra;

namespace SimpleDatabase.Planning.Optimisation.Tasks;

public class OptimiseInputsTask : IOptimiserTask
{
    private readonly Node _node;

    public OptimiseInputsTask(Node node)
    {
        _node = node;
    }

    public void Perform(Optimiser.Context context)
    {
        // throw new System.NotImplementedException();
    }
}