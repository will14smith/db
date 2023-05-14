using SimpleDatabase.Parsing.Expressions;
using SimpleDatabase.Schemas;

namespace SimpleDatabase.Planning.Nodes;

public class SeekIndexNode : Node
{
    public string TableName { get; }
    public TableIndex Index { get; }
    public string IndexName { get; }
    public Expression SeekPredicate { get; }
    
    public SeekIndexNode(string alias, string tableName, TableIndex index, Expression seekPredicate) : base(alias)
    {
        TableName = tableName;
        Index = index;
        IndexName = index.Name;
        SeekPredicate = seekPredicate;
    }
}