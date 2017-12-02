using System;
using System.Collections.Generic;
using System.Linq;
using SimpleDatabase.Execution;
using SimpleDatabase.Execution.Operations;
using SimpleDatabase.Execution.Operations.Columns;
using SimpleDatabase.Execution.Operations.Constants;
using SimpleDatabase.Execution.Operations.Functions;
using SimpleDatabase.Execution.Operations.Jumps;
using SimpleDatabase.Execution.Operations.Slots;
using SimpleDatabase.Parsing.Expressions;
using SimpleDatabase.Schemas.Types;

namespace SimpleDatabase.Planning.Iterators
{
    public class ConstantIterator : IIterator
    {
        private readonly IReadOnlyList<string> _columns;
        private readonly IReadOnlyList<IReadOnlyList<Expression>> _values;

        private readonly FunctionLabel _generator = FunctionLabel.Create();
        private readonly SlotLabel _generatorHandle = SlotLabel.Create();
        private readonly SlotLabel _current = SlotLabel.Create();

        public ConstantIterator(IReadOnlyList<string> columns, IReadOnlyList<IReadOnlyList<Expression>> values)
        {
            _columns = columns;
            _values = values;

            Functions = new Dictionary<FunctionLabel, Function>
            {
                { _generator, GenerateFunction() }
            };
            Outputs = columns.Select((x, i) => new IteratorOutput(new IteratorOutputName.Constant(x), null, new IOperation[]
            {
                new LoadOperation(_current), 
                new ColumnOperation(i), 
            })).ToList();
        }

        public IReadOnlyDictionary<SlotLabel, SlotDefinition> Slots => new Dictionary<SlotLabel, SlotDefinition>
        {
            { _generatorHandle, new SlotDefinition() },
            { _current, new SlotDefinition() },
        };
        public IReadOnlyDictionary<FunctionLabel, Function> Functions { get; }
        public IReadOnlyList<IteratorOutput> Outputs { get; }

        public IEnumerable<IOperation> Init(ProgramLabel emptyTarget)
        {
            if (!_values.Any())
            {
                yield return new JumpOperation(emptyTarget);
                yield break;
            }

            yield return new SetupCoroutineOperation(_generator, 0);
            yield return new StoreOperation(_generatorHandle);
            yield return new LoadOperation(_generatorHandle);
            yield return new CallCoroutineOperation(emptyTarget);
            yield return new StoreOperation(_current);
        }

        public IEnumerable<IOperation> MoveNext(ProgramLabel loopStartTarget)
        {
            var done = ProgramLabel.Create();

            yield return new LoadOperation(_generatorHandle);
            yield return new CallCoroutineOperation(done);
            yield return new StoreOperation(_current);
            yield return new JumpOperation(loopStartTarget);
            yield return done;
        }

        public IEnumerable<IOperation> Yield()
        {
            yield return new LoadOperation(_current);
            yield return new YieldOperation();
        }

        private Function GenerateFunction()
        {
            var operations = new List<IOperation>();

            foreach (var row in _values)
            {
                foreach (var value in row)
                {
                    var (ops, _) = CompileExpr(value);
                    operations.AddRange(ops);
                }

                operations.Add(new MakeRowOperation(row.Count));
                operations.Add(new ReturnOperation());
            }

            return new Function(operations, new Dictionary<SlotLabel, SlotDefinition>());
        }

        // TODO same as Compile in Projection iterator?
        private (IReadOnlyCollection<IOperation>, ColumnType) CompileExpr(Expression expr)
        {
            switch (expr)
            {
                case NumberLiteralExpression num:
                    return (new[] { new ConstIntOperation(num.Value), }, new ColumnType.Integer());

                case StringLiteralExpression str:
                    return (new[] { new ConstStringOperation(str.Value) }, new ColumnType.String(str.Value.Length));


                default: throw new ArgumentOutOfRangeException(nameof(expr), $"Unhandled type: {expr.GetType().FullName}");
            }
        }
    }
}