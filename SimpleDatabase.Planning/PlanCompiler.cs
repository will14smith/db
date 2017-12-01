using System;
using System.Collections.Generic;
using System.Linq;
using SimpleDatabase.Execution;
using SimpleDatabase.Execution.Operations;
using SimpleDatabase.Execution.Operations.Columns;
using SimpleDatabase.Execution.Operations.Cursors;
using SimpleDatabase.Execution.Operations.Slots;
using SimpleDatabase.Planning.Nodes;
using Table = SimpleDatabase.Schemas.Table;

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
            var root = plan.RootNode;

            // scan table: foreach(row in table) { result(row[0], row[1], ..., row[N]); }
            // project [ scan table ]: foreach(row in table) { result(row[p1], row[p2], ..., row[pN]); }
            // filter [ scan table ] P: foreach(row in table) { if(P(row)) continue; else result(row[0], row[1], ..., row[N]); }
            // project [ filter [ scan table ] P ]: foreach(row in table) { if(P(row)) continue; else result(row[p1], row[p2], ..., row[pN]); }

            // iterator:
            //   MoveNext
            //   Current
            
            // scan table:
            //   input: none (or the table?)
            //   output: row iterator
            //   iterator: 

            // filter:
            //   input: iterator (and predicate)
            //   output: iterator (filtered)

            // project:
            //   input: iterator (and projection)
            //   output: iterator (projected)

            var output = Compile(root);
            var operations = new List<IOperation>();

            foreach (var operation in output.Program.Operations)
            {
                if (operation is YieldRowOperation yield)
                {
                    operations.Add(operation);
                }
                else
                {
                    operations.Add(operation);
                }
            }

            return new Program(operations, output.Program.Slots);
        }

        private Output Compile(Node root)
        {
            switch (root)
            {
                case ScanTableNode scan: return Compile(scan);

                default: throw new ArgumentOutOfRangeException(nameof(root), $"Unhandled type: {root.GetType().FullName}");
            }
        }

        private Output Compile(ScanTableNode scan)
        {
            var table = _database.GetTable(scan.TableName);
            var columns = table.Columns;

            var cursor = SlotLabel.Create();
            var loop = ProgramLabel.Create();
            var finish = ProgramLabel.Create();

            var operations = new List<IOperation>();

            // tableCursor = open(table)
            operations.Add(new OpenReadOperation(_database.GetRootPage(table.Name)));
            operations.Add(new FirstOperation(finish));
            operations.Add(loop);
            operations.Add(new StoreOperation(cursor));

            // load columns
            for (var i = 0; i < columns.Count; i++)
            {
                operations.Add(new LoadOperation(cursor));
                operations.Add(new ColumnOperation(i));
            }

            // yield
            operations.Add(new YieldRowOperation(columns.Count));

            // next
            operations.Add(new LoadOperation(cursor));
            operations.Add(new NextOperation(loop));

            // done
            operations.Add(finish);
            operations.Add(new FinishOperation());

            var program = new Program(
                operations,
                new Dictionary<SlotLabel, SlotDefinition>
                {
                    { cursor, new SlotDefinition() }
                }
            );

            return new Output(
                program,
                new Dictionary<string, SlotLabel> { { scan.TableName, cursor } },
                columns.Select((x, i) => new OutputColumn(cursor, i)).ToList()
            );
        }
    }

    public class Output
    {
        public Program Program { get; }

        public IReadOnlyDictionary<string, SlotLabel> Cursors { get; }
        public IReadOnlyList<OutputColumn> Columns { get; }

        public Output(Program program, IReadOnlyDictionary<string, SlotLabel> cursors, IReadOnlyList<OutputColumn> columns)
        {
            Program = program;
            Cursors = cursors;
            Columns = columns;
        }
    }
    public class OutputColumn
    {
        public SlotLabel Cursor { get; }
        public int ColumnIndex { get; }

        public OutputColumn(SlotLabel cursor, int columnIndex)
        {
            Cursor = cursor;
            ColumnIndex = columnIndex;
        }
    }

    public class Database
    {
        private readonly IReadOnlyDictionary<string, Table> _tables;
        private readonly IReadOnlyDictionary<string, int> _tableRootPages;

        public Database(IReadOnlyDictionary<string, Table> tables, IReadOnlyDictionary<string, int> tableRootPages)
        {
            _tables = tables;
            _tableRootPages = tableRootPages;
        }

        public Table GetTable(string name) { return _tables[name]; }
        public int GetRootPage(string name) { return _tableRootPages[name]; }
    }

    public class ScanTableIterator
    {
        private readonly Database _database;
        private readonly Table _table;

        private readonly SlotLabel _cursor = SlotLabel.Create();

        public ScanTableIterator(Database database, Table table)
        {
            _database = database;
            _table = table;
        }

        public IEnumerable<IOperation> Init(ProgramLabel emptyTarget)
        {
            yield return new OpenReadOperation(_database.GetRootPage(_table.Name));
            yield return new FirstOperation(emptyTarget);
            yield return new StoreOperation(_cursor);
        }

        public IEnumerable<IOperation> MoveNext(ProgramLabel loopStartTarget)
        {
            yield return new NextOperation(loopStartTarget);
        }

        public IEnumerable<IOperation> Yield()
        {
            var columns = _table.Columns;

            for (var i = 0; i < columns.Count; i++)
            {
                yield return new LoadOperation(_cursor);
                yield return new ColumnOperation(i);
            }

            yield return new YieldRowOperation(columns.Count);
        }
    }
}
