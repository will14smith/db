using System.Collections.Generic;
using SimpleDatabase.Execution;
using SimpleDatabase.Execution.Operations;
using SimpleDatabase.Execution.Operations.Cursors;
using SimpleDatabase.Planning.Items;
using SimpleDatabase.Schemas;

namespace SimpleDatabase.Planning.Iterators
{
    public class ScanTableIterator : IIterator
    {
        private readonly IOperationGenerator _generator;
        private readonly Table _table;
        private readonly bool _writable;

        private readonly SlotItem _cursor;

        public ScanTableIterator(IOperationGenerator generator, Table table, bool writable)
        {
            _generator = generator;
            _table = table;
            _writable = writable;

            _cursor = new SlotItem(_generator.NewSlot(new SlotDefinition($"cursor_table_scan_{table.Name}")));

            Output = ComputeOutput();
        }

        public IteratorOutput Output { get; }

        public void GenerateInit()
        {
            _generator.Emit(_writable ? (IOperation)new OpenWriteOperation(_table) : new OpenReadTableOperation(_table));
            _generator.Emit(new FirstOperation());
            _cursor.Store(_generator);
        }

        public void GenerateMoveNext(ProgramLabel loopStart, ProgramLabel loopEnd)
        {
            _cursor.Load(_generator);
            _generator.Emit(new NextOperation(loopEnd));
            _cursor.Store(_generator);
        }

        public void Reset()
        {
            throw new System.NotImplementedException();
        }

        private IteratorOutput ComputeOutput()
        {
            var columns = new List<IteratorOutput.Named>();

            for (var index = 0; index < _table.Columns.Count; index++)
            {
                var column = _table.Columns[index];
                var name = new IteratorOutputName.TableColumn(_table.Name, column.Name);
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