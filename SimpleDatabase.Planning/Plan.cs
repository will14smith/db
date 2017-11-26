using SimpleDatabase.Planning.Nodes;

namespace SimpleDatabase.Planning
{
    public class Plan
    {
        public Node RootNode { get; }

        public Plan(Node rootNode)
        {
            RootNode = rootNode;
        }
    }
}