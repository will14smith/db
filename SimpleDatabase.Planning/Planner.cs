using System;
using System.Linq;
using SimpleDatabase.Parsing.Statements;
using SimpleDatabase.Planning.Nodes;
using SimpleDatabase.Storage;

namespace SimpleDatabase.Planning
{
    public class Planner
    {
        private readonly Database _database;

        public Planner(Database database)
        {
            _database = database;
        }

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

                        if (select.Ordering.Any())
                        {
                            root = new SortNode(root, select.Ordering);
                        }

                        return new Plan(root);
                    }

                case InsertStatement insert:
                    {
                        var columns = insert.Columns;
                        if (!columns.Any())
                        {
                            var table = _database.GetTable(insert.Table);
                            columns = table.Table.Columns.Select(x => x.Name).ToList();
                        }

                        var input = new ConstantNode(columns, insert.Values);

                        var root = new InsertNode(insert.Table, input);

                        return new Plan(root);
                    }

                case DeleteStatement delete:
                    {
                        Node root = new ScanTableNode(delete.Table);
                        root = delete.Where.Map(
                            pred => new FilterNode(root, pred),
                            () => root
                        );
                        root = new DeleteNode(root);

                        return new Plan(root);
                    }

                default: throw new ArgumentOutOfRangeException(nameof(statment), $"Unhandled type: {statment.GetType().FullName}");
            }
        }
    }
}
