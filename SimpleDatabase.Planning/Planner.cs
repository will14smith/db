using System;
using SimpleDatabase.Parsing.Statements;
using SimpleDatabase.Planning.Nodes;

namespace SimpleDatabase.Planning
{
    public class Planner
    {
        public Plan Plan(Statement statment)
        {
            if (!(statment is SelectStatement select))
            {
                throw new NotImplementedException();
            }

            Node root = new ScanTableNode(((Table.TableName) select.Table).Name);
            root = select.Where.Map(
                pred => new FilterNode(root, pred),
                () => root
            );
            root = new ProjectionNode(root, select.Columns);

            return new Plan(root);
        }
    }
}
