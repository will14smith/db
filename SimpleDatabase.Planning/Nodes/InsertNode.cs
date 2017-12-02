namespace SimpleDatabase.Planning.Nodes
{
    public class InsertNode : Node
    {
        public string TableName { get; }
        public Node Input { get; }

        public InsertNode(string tableName, Node input)
        {
            TableName = tableName;
            Input = input;
        }
    }
}
