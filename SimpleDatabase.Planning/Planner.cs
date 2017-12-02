using System;
using SimpleDatabase.Parsing.Statements;
using SimpleDatabase.Planning.Nodes;

namespace SimpleDatabase.Planning
{
    public class Planner
    {
        public Plan Plan(Statement statment)
        {
            switch (statment)
            {
                case SelectStatement select:
                    {
                        Node root = new ScanTableNode(((Table.TableName)select.Table).Name);
                        root = select.Where.Map(
                            pred => new FilterNode(root, pred),
                            () => root
                        );
                        root = new ProjectionNode(root, select.Columns);

                        return new Plan(root);
                    }

                case InsertStatement insert:
                    {
                        var input = new ConstantNode(insert.Columns, insert.Values);

                        var root = new InsertNode(insert.Table, input);

                        return new Plan(root);
                    }

                default: throw new ArgumentOutOfRangeException(nameof(statment), $"Unhandled type: {statment.GetType().FullName}");
            }
        }
    }
}
