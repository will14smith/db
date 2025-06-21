using SimpleDatabase.Planning.Optimisation.Algebra;

namespace SimpleDatabase.Planning.Optimisation.Patterns;

public interface IPattern
{
    public bool Matches(Node node);
}

