namespace SimpleDatabase.Planning.Nodes
{
    public class ScanTableNode : Node
    {
        public string TableName { get; }

        public ScanTableNode(string tableName)
        {
            TableName = tableName;
        }
    }
}
