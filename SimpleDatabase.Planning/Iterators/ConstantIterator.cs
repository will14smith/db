using System;
using System.Collections.Generic;
using System.Linq;
using SimpleDatabase.Execution;
using SimpleDatabase.Execution.Operations;
using SimpleDatabase.Execution.Operations.Functions;
using SimpleDatabase.Execution.Operations.Jumps;
using SimpleDatabase.Parsing.Expressions;
using SimpleDatabase.Planning.Items;

namespace SimpleDatabase.Planning.Iterators
{
    public class ConstantIterator : IIterator
    {
        private readonly IOperationGenerator _generator;
        private readonly IReadOnlyList<string> _columns;
        private readonly IReadOnlyList<IReadOnlyList<Expression>> _values;

        private readonly FunctionLabel _itemSource;
        private readonly SlotItem _itemSourceHandle;
        private readonly SlotItem _current;

        public ConstantIterator(IOperationGenerator generator, IReadOnlyList<string> columns, IReadOnlyList<IReadOnlyList<Expression>> values)
        {
            _generator = generator;
            _columns = columns;
            _values = values;

            _itemSource = GenerateFunction();
            _itemSourceHandle = new SlotItem(_generator.NewSlot(new SlotDefinition("item source handle")));
            _current = new SlotItem(_generator.NewSlot(new SlotDefinition("item source current")));

            Output = GenerateOutput();
        }

        public IteratorOutput Output { get; }

        public void GenerateInit(ProgramLabel emptyTarget)
        {
            if (!_values.Any())
            {
                _generator.Emit(new JumpOperation(emptyTarget));
                return;
            }

            _generator.Emit(new SetupCoroutineOperation(_itemSource, 0));
            _itemSourceHandle.Store(_generator);

            _itemSourceHandle.Load(_generator);
            _generator.Emit(new CallCoroutineOperation(emptyTarget));
            _current.Store(_generator);
        }

        public void GenerateMoveNext(ProgramLabel loopStartTarget)
        {
            var done = _generator.NewLabel("done");

            _itemSourceHandle.Load(_generator);
            _generator.Emit(new CallCoroutineOperation(done));
            _current.Store(_generator);

            _generator.Emit(new JumpOperation(loopStartTarget));
            _generator.MarkLabel(done);
        }

        private IteratorOutput GenerateOutput()
        {
            var columns = new List<IteratorOutput.Named>();

            for (var index = 0; index < _columns.Count; index++)
            {
                var column = _columns[index];

                columns.Add(new IteratorOutput.Named(
                    new IteratorOutputName.Constant(column),
                    new ColumnItem(_current, index)
                ));
            }

            // TODO _current technically isn't a cursor...
            return new IteratorOutput.Row(_current, columns);
        }

        private FunctionLabel GenerateFunction()
        {
            var generator = _generator.NewFunction();

            foreach (var row in _values)
            {
                foreach (var value in row)
                {
                    CompileExpr(value).Load(generator);
                }

                generator.Emit(new MakeRowOperation(row.Count));
                generator.Emit(new ReturnOperation());
            }

            return generator.Label;
        }

        // TODO same as Compile in Projection iterator?
        private Item CompileExpr(Expression expr)
        {
            switch (expr)
            {
                case NumberLiteralExpression num:
                    return new ConstItem(num.Value);

                case StringLiteralExpression str:
                    return new ConstItem(str.Value);

                default: throw new ArgumentOutOfRangeException(nameof(expr), $"Unhandled type: {expr.GetType().FullName}");
            }
        }
    }
}