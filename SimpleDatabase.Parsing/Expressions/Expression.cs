namespace SimpleDatabase.Parsing.Expressions
{
    public abstract class Expression
    {
        public abstract override bool Equals(object obj);
        public abstract override int GetHashCode();
    }
}
