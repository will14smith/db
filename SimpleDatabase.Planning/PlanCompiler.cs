using System;
using SimpleDatabase.Execution;
using SimpleDatabase.Execution.Operations;
using SimpleDatabase.Execution.Operations.Jumps;
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
            var program = new ProgramGenerator();

            var generator = program.NewFunction();

            var iter = Compile(plan.RootNode, generator, false);

            var start = generator.NewLabel("loop start");
            var done = generator.NewLabel("loop done");

            iter.GenerateInit();
            generator.MarkLabel(start);
            iter.GenerateMoveNext(start, done);
            Yield(generator, iter.Output);
            generator.Emit(new JumpOperation(start));
            generator.MarkLabel(done);
            generator.Emit(new FinishOperation());

            return program.CreateProgram(generator);
        }

        private void Yield(IOperationGenerator generator, IteratorOutput output)
        {
            if (output is IteratorOutput.Void)
            {
                return;
            }

            output.Load(generator);
            generator.Emit(new YieldOperation());
        }

        private IIterator Compile(Node node, IOperationGenerator generator, bool writable)
        {
            switch (node)
            {
                case ConstantNode constant: return new ConstantIterator(generator, constant.Columns, constant.Values);
                case ScanTableNode scan: return new ScanTableIterator(generator, _database.GetTable(scan.TableName), writable);
                case ProjectionNode projection: return new ProjectionIterator(Compile(projection.Input, generator, writable), projection.Columns);
                case FilterNode filter: return new FilterIterator(generator, Compile(filter.Input, generator, writable), filter.Predicate);
                case InsertNode insert: return new InsertIterator(generator, Compile(insert.Input, generator, writable), _database.GetTable(insert.TableName));
                case DeleteNode delete: return new DeleteIterator(generator, Compile(delete.Input, generator, true));

                default: throw new ArgumentOutOfRangeException(nameof(node), $"Unhandled type: {node.GetType().FullName}");
            }
        }
    }
}
