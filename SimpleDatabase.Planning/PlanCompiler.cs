using System;
using System.Collections.Generic;
using System.Linq;
using SimpleDatabase.Execution;
using SimpleDatabase.Execution.Operations;
using SimpleDatabase.Planning.Iterators;
using SimpleDatabase.Planning.Nodes;
using SimpleDatabase.Storage;

namespace SimpleDatabase.Planning
{
    public class PlanCompiler
    {
        private readonly Database _database;

        public PlanCompiler(Database database)
        {
            _database = database;
        }

        public Program Compile(Plan plan)
        {
            var iter = Compile(plan.RootNode);

            var start = ProgramLabel.Create();
            var done = ProgramLabel.Create();

            var operations = new List<IOperation>();

            operations.AddRange(iter.Init(done));
            operations.Add(start);
            operations.AddRange(iter.Outputs.SelectMany(x => x.LoadOperations));
            operations.AddRange(iter.MoveNext(start));
            operations.Add(done);
            operations.Add(new FinishOperation());

            return new Program(operations, iter.Slots);
        }

        private IIterator Compile(Node node)
        {
            switch (node)
            {
                case ScanTableNode scan: return new ScanTableIterator(_database, _database.GetTable(scan.TableName).Table);
                case ProjectionNode projection: return new ProjectionIterator(Compile(projection.Input), projection.Columns);
                case FilterNode filter: return new FilterIterator(Compile(filter.Input), filter.Predicate);

                default: throw new ArgumentOutOfRangeException(nameof(node), $"Unhandled type: {node.GetType().FullName}");
            }
        }
    }
}
