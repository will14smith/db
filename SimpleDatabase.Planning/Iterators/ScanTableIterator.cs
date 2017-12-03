using System.Collections.Generic;
using SimpleDatabase.Execution;
using SimpleDatabase.Execution.Operations;
using SimpleDatabase.Execution.Operations.Cursors;
using SimpleDatabase.Planning.Items;
using SimpleDatabase.Storage;

namespace SimpleDatabase.Planning.Iterators
{
    public class ScanTableIterator : IIterator
    {
        private readonly IOperationGenerator _generator;
        private readonly StoredTable _table;
        private readonly bool _writable;

        private readonly SlotItem _cursor;

        public ScanTableIterator(IOperationGenerator generator, StoredTable table, bool writable)
        {
            _generator = generator;
            _table = table;
            _writable = writable;

            _cursor = new SlotItem(_generator.NewSlot(new SlotDefinition("cursor")));

            Output = ComputeOutput();
        }

        public IteratorOutput Output { get; }

        public void GenerateInit()
        {
            _generator.Emit(_writable ? (IOperation)new OpenWriteOperation(_table) : new OpenReadOperation(_table));
            _generator.Emit(new FirstOperation());
            _cursor.Store(_generator);
        }

        public void GenerateMoveNext(ProgramLabel loopStart, ProgramLabel loopEnd)
        {
            _cursor.Load(_generator);
            _generator.Emit(new NextOperation(loopEnd, true));
            _cursor.Store(_generator);
        }

        private IteratorOutput ComputeOutput()
        {
            var columns = new List<IteratorOutput.Named>();

            for (var index = 0; index < _table.Table.Columns.Count; index++)
            {
                var column = _table.Table.Columns[index];
                var name = new IteratorOutputName.TableColumn(_table.Table.Name, column.Name);
                var value = new ColumnItem(_cursor, index);

                columns.Add(new IteratorOutput.Named(name, value));
            }

            return new IteratorOutput.Row(
                _cursor,
                columns
            );
        }
    }
}