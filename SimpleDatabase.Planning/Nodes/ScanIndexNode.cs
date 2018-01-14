namespace SimpleDatabase.Planning.Nodes
{
    public class ScanIndexNode : Node
    {
        public string TableName { get; }
        public string IndexName { get; }

        public ScanIndexNode(string tableName, string indexName)
        {
            TableName = tableName;
            IndexName = indexName;
        }
    }
}