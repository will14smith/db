using System;
using System.Linq;
using SimpleDatabase.Parsing.Statements;
using SimpleDatabase.Planning.Nodes;
using SimpleDatabase.Storage;

namespace SimpleDatabase.Planning
{
    public partial class Planner
    {
        private readonly Database _database;

        public Planner(Database database)
        {
            _database = database;
        }

        public Plan Plan(StatementDataManipulation statement)
        {
            var aliasCounter = 0;
            
            switch (statement)
            {
                case SelectStatement select:
                    return PlanSelect(select);

                case InsertStatement insert:
                    {
                        var columns = insert.Columns;
                        if (!columns.Any())
                        {
                            var table = _database.GetTable(insert.Table);
                            columns = table.Columns.Select(x => x.Name).ToList();
                        }

                        var input = new ConstantNode($"c{aliasCounter++}", columns, insert.Values);

                        var root = new InsertNode($"i{aliasCounter++}", insert.Table, input);

                        return new Plan(root);
                    }

                case DeleteStatement delete:
                    {
                        Node root = new ScanTableNode($"t{aliasCounter++}", delete.Table);
                        root = delete.Where.Map(
                            pred => new FilterNode($"f{aliasCounter++}", root, pred),
                            () => root
                        );
                        root = new DeleteNode($"d{aliasCounter++}", root);

                        return new Plan(root);
                    }

                case ExplainStatement explain:
                {
                    var innerPlanNode = Plan(explain.Statement).RootNode;
                    
                    var root = new ExplainNode($"e{aliasCounter++}", innerPlanNode);
                    return new Plan(root);
                }
                
                default: throw new ArgumentOutOfRangeException(nameof(statement), $"Unhandled type: {statement.GetType().FullName}");
            }
        }
    }
}
