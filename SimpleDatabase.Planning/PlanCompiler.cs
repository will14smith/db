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
            operations.AddRange(iter.Yield());
            operations.AddRange(iter.MoveNext(start));
            operations.Add(done);
            operations.Add(new FinishOperation());

            var main = new Function(operations, iter.Slots);
            var mainLabel = FunctionLabel.Create();

            var functions = iter.Functions.ToDictionary(x => x.Key, x => x.Value);
            functions.Add(mainLabel, main);

            return new Program(mainLabel, functions);
        }

        private IIterator Compile(Node node)
        {
            switch (node)
            {
                case ConstantNode constant: return new ConstantIterator(constant.Columns, constant.Values);
                case ScanTableNode scan: return new ScanTableIterator(_database.GetTable(scan.TableName));
                case ProjectionNode projection: return new ProjectionIterator(Compile(projection.Input), projection.Columns);
                case FilterNode filter: return new FilterIterator(Compile(filter.Input), filter.Predicate);
                case InsertNode insert: return new InsertIterator(Compile(insert.Input), _database.GetTable(insert.TableName));
                case DeleteNode delete: return new DeleteIterator(Compile(delete.Input));

                default: throw new ArgumentOutOfRangeException(nameof(node), $"Unhandled type: {node.GetType().FullName}");
            }
        }
    }
}
