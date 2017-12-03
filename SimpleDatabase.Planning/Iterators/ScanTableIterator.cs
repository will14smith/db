using System.Collections.Generic;
using SimpleDatabase.Execution;
using SimpleDatabase.Execution.Operations.Cursors;
using SimpleDatabase.Execution.Operations.Jumps;
using SimpleDatabase.Planning.Items;
using SimpleDatabase.Storage;

namespace SimpleDatabase.Planning.Iterators
{
    public class ScanTableIterator : IIterator
    {
        private readonly IOperationGenerator _generator;
        private readonly StoredTable _table;

        private readonly SlotItem _cursor;

        public ScanTableIterator(IOperationGenerator generator, StoredTable table)
        {
            _generator = generator;
            _table = table;

            var cursorLabel = _generator.NewSlot(new SlotDefinition());
            _cursor = new SlotItem(generator, cursorLabel);

            Output = ComputeOutput();
        }

        public IteratorOutput Output { get; }

        public void GenerateInit(ProgramLabel emptyTarget)
        {
            _generator.Emit(new OpenReadOperation(_table));
            _generator.Emit(new FirstOperation(emptyTarget));
            _cursor.Store();
        }

        public void GenerateMoveNext(ProgramLabel loopStartTarget)
        {
            var s = _generator.NewLabel();
            var e = _generator.NewLabel();

            _cursor.Load();
            _generator.Emit(new NextOperation(s));
            _generator.Emit(new JumpOperation(e));
            _generator.MarkLabel(s);
            _cursor.Store();
            _generator.Emit(new JumpOperation(loopStartTarget));
            _generator.MarkLabel(e);
        }

        private IteratorOutput ComputeOutput()
        {
            var columns = new List<IteratorOutput.Named>();

            for (var index = 0; index < _table.Table.Columns.Count; index++)
            {
                var column = _table.Table.Columns[index];
                var name = new IteratorOutputName.TableColumn(_table.Table.Name, column.Name);
                var value = new ColumnItem(_generator, _cursor, index);

                columns.Add(new IteratorOutput.Named(name, value));
            }

            return new IteratorOutput.Row(
                _cursor,
                columns
            );
        }
    }
}