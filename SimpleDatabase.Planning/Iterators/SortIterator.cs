using System;
using System.Collections.Generic;
using System.Linq;
using SimpleDatabase.Execution;
using SimpleDatabase.Execution.Operations;
using SimpleDatabase.Execution.Operations.Cursors;
using SimpleDatabase.Execution.Operations.Jumps;
using SimpleDatabase.Execution.Operations.Sorting;
using SimpleDatabase.Parsing.Expressions;
using SimpleDatabase.Parsing.Statements;
using SimpleDatabase.Planning.Items;
using SimpleDatabase.Schemas;

namespace SimpleDatabase.Planning.Iterators
{
    // TODO could optimise by sharing key & data columns where possible.
    public class SortIterator : IIterator
    {
        private readonly IOperationGenerator _generator;
        private readonly IIterator _inner;
        private readonly IReadOnlyList<OrderExpression> _orderings;

        private readonly SlotItem _cursor;
        private readonly SlotItem _sorter;
        private readonly KeyStructure _key;

        public SortIterator(IOperationGenerator generator, IIterator inner, IReadOnlyList<OrderExpression> orderings)
        {
            _generator = generator;
            _inner = inner;
            _orderings = orderings;

            _cursor = new SlotItem(_generator.NewSlot(new SlotDefinition("sorter cursor")));
            _sorter = new SlotItem(_generator.NewSlot(new SlotDefinition("sorter")));

            _key = GenerateKey();
            Output = GenerateOutput();
        }

        public IteratorOutput Output { get; }

        public void GenerateInit()
        {
            var innerOutputRow = (IteratorOutput.Row)_inner.Output;

            // create sorter
            _generator.Emit(new SorterNew(_key));
            _sorter.Store(_generator);

            // insert all items (w.r.t. key)
            _inner.GenerateInit();

            var start = _generator.NewLabel("sorter insert start");
            var end = _generator.NewLabel("sorter insert end");

            _generator.MarkLabel(start);
            _inner.GenerateMoveNext(start, end);

            foreach (var col in _orderings)
            {
                CompileExpr(innerOutputRow, col.Expression).Load(_generator);
            }
            foreach (var col in innerOutputRow.Columns)
            {
                col.Load(_generator);
            }
            _generator.Emit(new MakeRowOperation(_key.Keys.Count + _key.Data.Count));
            _sorter.Load(_generator);
            _generator.Emit(new InsertOperation());

            _generator.Emit(new JumpOperation(start));
            _generator.MarkLabel(end);

            // sort sorter
            _sorter.Load(_generator);
            _generator.Emit(new SorterSort());

            // create a cursor at the start at of the sorter
            _sorter.Load(_generator);
            _generator.Emit(new SorterCursor());
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
            throw new NotImplementedException();
        }

        private KeyStructure GenerateKey()
        {
            var innerOutputRow = (IteratorOutput.Row)_inner.Output;

            var keys = new List<(Column, KeyOrdering)>();
            var data = new List<Column>();

            // TODO these are "fake" columns, just to match the counts
            // TODO the types are null...
            for(var i = 0; i < _orderings.Count; i++) { keys.Add((new Column($"key{i}", null), _orderings[i].Order == Order.Ascending ? KeyOrdering.Ascending : KeyOrdering.Descending)); }
            for(var i = 0; i < innerOutputRow.Columns.Count; i++) { data.Add(new Column($"data{i}", null)); }

            return new KeyStructure(keys, data);
        }

        private IteratorOutput GenerateOutput()
        {
            var offset = _orderings.Count;
            var innerOutputRow = (IteratorOutput.Row)_inner.Output;

            var columns = innerOutputRow.Columns.Select((x, i) => new IteratorOutput.Named(x.Name, new ColumnItem(_cursor, i + offset))).ToList();

            return new IteratorOutput.Row(
                _cursor,
                columns
            );
        }

        private Item CompileExpr(IteratorOutput.Row input, Expression expr)
        {
            switch (expr)
            {
                case ColumnNameExpression column:
                    var result = input.Columns.Single(x => x.Name.Matches(column.Name));

                    return result.Value;

                default: throw new ArgumentOutOfRangeException(nameof(expr), $"Unhandled type: {expr.GetType().FullName}");
            }
        }

    }
}
