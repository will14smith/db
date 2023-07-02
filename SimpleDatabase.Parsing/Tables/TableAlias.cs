using System;

namespace SimpleDatabase.Parsing.Tables;

public class TableAlias
{
    public string Name { get; }
    public string Alias { get; }

    public TableAlias(string name) : this(name, name) { }
    
    public TableAlias(string name, string alias)
    {
        Name = name;
        Alias = alias;
    }

    public bool Equals(TableAlias? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name && Alias == other.Alias;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((TableAlias)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Alias);
    }

    public static bool operator ==(TableAlias? left, TableAlias? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(TableAlias? left, TableAlias? right)
    {
        return !Equals(left, right);
    }
}