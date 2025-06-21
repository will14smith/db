namespace SimpleDatabase.Planning.Optimisation.Algebra;

public record AlgebraColumn(string Name, AlgebraExpression Value)
{
    public override string ToString()
    {
        return $"{Name}: {Value}";
    }
}