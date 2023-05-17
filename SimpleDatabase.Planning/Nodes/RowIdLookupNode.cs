using SimpleDatabase.Parsing.Expressions;

namespace SimpleDatabase.Planning.Nodes;

public class RowIdLookupNode : Node
{
    public string TableName { get; }
    public Expression RowId { get; }

    public RowIdLookupNode(string alias, string tableName, Expression rowId) : base(alias)
    {
        TableName = tableName;
        RowId = rowId;
    }
}