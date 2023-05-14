namespace SimpleDatabase.Planning.Nodes
{
    public class InsertNode : Node
    {
        public string TableName { get; }
        public Node Input { get; }

        public InsertNode(string alias, string tableName, Node input) : base(alias)
        {
            TableName = tableName;
            Input = input;
        }
    }
}
