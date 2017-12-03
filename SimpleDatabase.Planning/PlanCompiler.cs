using System;
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
            var generator = new OperationGenerator();

            var iter = Compile(plan.RootNode, generator);

            var start = generator.NewLabel();
            var done = generator.NewLabel();
            
            iter.GenerateInit(done);
            generator.MarkLabel(start);
            Yield(generator, iter.Output);
            iter.GenerateMoveNext(start);
            generator.MarkLabel(done);
            generator.Emit(new FinishOperation());

            return generator.CreateProgram();
        }

        private void Yield(OperationGenerator generator, IteratorOutput output)
        {
            switch (output)
            {
                case IteratorOutput.Row row:
                    foreach (var column in row.Columns)
                    {
                        column.Value.Load();
                    }

                    generator.Emit(new MakeRowOperation(row.Columns.Count));
                    generator.Emit(new YieldOperation());

                    break;

                default: throw new ArgumentOutOfRangeException(nameof(output), $"Unhandled type: {output.GetType().FullName}");
            }
        }

        private IIterator Compile(Node node, IOperationGenerator generator)
        {
            switch (node)
            {
                case ConstantNode constant: return new ConstantIterator(constant.Columns, constant.Values);
                case ScanTableNode scan: return new ScanTableIterator(generator, _database.GetTable(scan.TableName));
                case ProjectionNode projection: return new ProjectionIterator(Compile(projection.Input, generator), projection.Columns);
                case FilterNode filter: return new FilterIterator(generator, Compile(filter.Input, generator), filter.Predicate);
                case InsertNode insert: return new InsertIterator(Compile(insert.Input, generator), _database.GetTable(insert.TableName));
                case DeleteNode delete: return new DeleteIterator(Compile(delete.Input, generator));

                default: throw new ArgumentOutOfRangeException(nameof(node), $"Unhandled type: {node.GetType().FullName}");
            }
        }
    }
}
