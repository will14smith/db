namespace SimpleDatabase.Planning.Optimisation.Algebra;

public abstract record AlgebraExpression
{
    public record And(AlgebraExpression Left, AlgebraExpression Right) : AlgebraExpression
    {
        public override string ToString() => $"({Left} AND {Right})";
    }
    
    public record Equality(AlgebraExpression Left, AlgebraExpression Right) : AlgebraExpression
    {
        public override string ToString() => $"({Left} = {Right})";
    }
    
    public record Column(string Alias, string ColumnName) : AlgebraExpression
    {
        public override string ToString() => $"{Alias}.{ColumnName}";
    }

    public record NumberLiteral(int Value) : AlgebraExpression
    {
        public override string ToString() => $"{Value}";
    }
}