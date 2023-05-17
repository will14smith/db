using System;
using System.Collections.Generic;
using SimpleDatabase.Planning.Items;
using SimpleDatabase.Planning.Iterators;

namespace SimpleDatabase.Planning;

public class Scope
{
    public static Scope Initial => new(null);

    private readonly Scope? _parent;
    private readonly Dictionary<string, IteratorOutput> _values = new();

    private Scope(Scope? parent)
    {
        _parent = parent;
    }

    public IteratorOutput Get(ScopeVariable variable)
    {
        switch (variable)
        {
            case ScopeVariable.Named named:
                if (_values.TryGetValue(named.Name, out var namedValue))
                {
                    return namedValue;
                }

                if (_parent is not null)
                {
                    return _parent.Get(named);
                }
                
                throw new Exception($"variable '{named.Name}' was not found");

            case ScopeVariable.Indexed indexed:
                var indexedSource = Get(indexed.Source);

                if (indexedSource is IteratorOutput.Row indexedRow)
                {
                    return indexedRow.Columns[indexed.Index];
                }
                
                throw new Exception($"variable '{indexed.Source}' was not indexable");
            
            default: throw new ArgumentOutOfRangeException(nameof(variable));
        }
    }

    public void Set(ScopeVariable.Named variable, IteratorOutput value)
    {
        _values.Add(variable.Name, value);
    }
    
    public Scope Push()
    {
        return new Scope(this);
    }
}

public abstract class ScopeVariable
{
    public class Named : ScopeVariable
    {
        public string Name { get; }

        public Named(string name)
        {
            Name = name;
        }
    }

    public class Indexed : ScopeVariable
    {
        public ScopeVariable Source { get; }
        public int Index { get; }

        public Indexed(ScopeVariable source, int index)
        {
            Source = source;
            Index = index;
        }
    }
}