namespace SimpleDatabase.Planning.Nodes
{
    public class ScanTableNode : Node
    {
        public string TableName { get; }

        public ScanTableNode(string alias, string tableName) : base(alias)
        {
            TableName = tableName;
        }
    }
}
