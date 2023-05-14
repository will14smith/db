namespace SimpleDatabase.Planning.Nodes
{
    public class ScanIndexNode : Node
    {
        public string TableName { get; }
        public string IndexName { get; }

        public ScanIndexNode(string alias, string tableName, string indexName) : base(alias)
        {
            TableName = tableName;
            IndexName = indexName;
        }
    }
}