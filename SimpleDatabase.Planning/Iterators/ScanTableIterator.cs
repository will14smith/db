using System;
using System.Collections.Generic;
using System.Linq;
using SimpleDatabase.Execution;
using SimpleDatabase.Execution.Operations;
using SimpleDatabase.Execution.Operations.Columns;
using SimpleDatabase.Execution.Operations.Cursors;
using SimpleDatabase.Execution.Operations.Slots;
using SimpleDatabase.Schemas;

namespace SimpleDatabase.Planning.Iterators
{
    public class ScanTableIterator : IIterator
    {
        private readonly Database _database;
        private readonly Table _table;

        private readonly SlotLabel _cursor = SlotLabel.Create();

        public ScanTableIterator(Database database, Table table)
        {
            _database = database;
            _table = table;

            Outputs = ComputeOutputs();
        }

        public IReadOnlyDictionary<SlotLabel, SlotDefinition> Slots => new Dictionary<SlotLabel, SlotDefinition>
        {
            { _cursor, new SlotDefinition() }
        };
        public IReadOnlyList<IteratorOutput> Outputs { get; }

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

        private IReadOnlyList<IteratorOutput> ComputeOutputs()
        {
            return _table.Columns.Select((x, i) => new IteratorOutput(
                new IteratorOutputName.TableColumn(_table.Name, x.Name),
                x.Type,
                new IOperation[] { new LoadOperation(_cursor), new ColumnOperation(i) })).ToList();
        }
    }
}