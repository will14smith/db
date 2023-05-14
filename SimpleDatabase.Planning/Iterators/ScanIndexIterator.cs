using System;
using System.Collections.Generic;
using SimpleDatabase.Execution;
using SimpleDatabase.Execution.Operations.Cursors;
using SimpleDatabase.Planning.Items;
using SimpleDatabase.Schemas;

namespace SimpleDatabase.Planning.Iterators
{
    public class ScanIndexIterator : IIterator
    {
        private readonly IOperationGenerator _generator;
        private readonly Table _table;
        private readonly TableIndex _index;
        private readonly bool _writable;

        private readonly SlotItem _cursor;

        public ScanIndexIterator(IOperationGenerator generator, Table table, TableIndex index, bool writable)
        {
            _generator = generator;
            _table = table;
            _index = index;
            _writable = writable;

            _cursor = new SlotItem(_generator.NewSlot(new SlotDefinition($"cursor_index_scan_{index.Name}")));

            Output = ComputeOutput();
        }

        public IteratorOutput Output { get; }

        public void GenerateInit()
        {
            if (_writable)
            {
                throw new NotImplementedException();
            }

            _generator.Emit(new OpenReadIndexOperation(_table, _index));
            _generator.Emit(new FirstOperation());
            _cursor.Store(_generator);
        }

        public void GenerateMoveNext(ProgramLabel loopStart, ProgramLabel loopEnd)
        {
            _cursor.Load(_generator);
            _generator.Emit(new NextOperation(loopEnd));
            _cursor.Store(_generator);
        }

        private IteratorOutput ComputeOutput()
        {
            var columns = new List<IteratorOutput.Named>();
        
            // column 0 is the rowid
            var columnIndex = 1;
        
            foreach (var column in _index.Structure.Keys)
            {
                var name = new IteratorOutputName.TableColumn(_table.Name, column.Item1.Name);
                var value = new ColumnItem(_cursor, columnIndex++);

                columns.Add(new IteratorOutput.Named(name, value));
            }
        
            foreach (var column in _index.Structure.Data)
            {
                var name = new IteratorOutputName.TableColumn(_table.Name, column.Name);
                var value = new ColumnItem(_cursor, columnIndex++);

                columns.Add(new IteratorOutput.Named(name, value));
            }

            return new IteratorOutput.Row(
                _cursor,
                columns
            );
        }
    }
}