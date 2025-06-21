namespace SimpleDatabase.Planning.Optimisation.Tasks;

public interface IOptimiserTask
{
    void Perform(Optimiser.Context context);
}