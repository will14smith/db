using System.Collections.Generic;
using SimpleDatabase.Planning.Optimisation.Algebra;

namespace SimpleDatabase.Planning.Optimisation;

public class Group
{
    private readonly List<Node> _nodes = new();
    public IReadOnlyList<Node> Nodes => _nodes;
    public int Id { get; }
    
    public Group(int id)
    {
        Id = id;
    }
    
    public void Add(Node node)
    {
        _nodes.Add(node);
    }
}