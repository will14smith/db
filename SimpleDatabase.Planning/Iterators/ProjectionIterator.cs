using System;
using System.Collections.Generic;
using System.Linq;
using SimpleDatabase.Execution;
using SimpleDatabase.Execution.Operations;
using SimpleDatabase.Parsing.Expressions;
using SimpleDatabase.Parsing.Statements;
using SimpleDatabase.Schemas.Types;

namespace SimpleDatabase.Planning.Iterators
{
    public class ProjectionIterator : IIterator
    {
        private readonly IIterator _input;
        private readonly IReadOnlyList<ResultColumn> _columns;

        public ProjectionIterator(IIterator input, IReadOnlyList<ResultColumn> columns)
        {
            _input = input;
            _columns = columns;

            Outputs = CalculateOutputs();
        }

        public IReadOnlyDictionary<SlotLabel, SlotDefinition> Slots => _input.Slots;
        public IReadOnlyList<IteratorOutput> Outputs { get; }

        public IEnumerable<IOperation> Init(ProgramLabel emptyTarget)
        {
            return _input.Init(emptyTarget);
        }

        public IEnumerable<IOperation> MoveNext(ProgramLabel loopStartTarget)
        {
            return _input.MoveNext(loopStartTarget);
        }

        public IEnumerable<IOperation> Yield()
        {
            foreach (var output in Outputs)
            {
                foreach (var op in output.LoadOperations)
                {
                    yield return op;
                }
            }

            yield return new MakeRowOperation(Outputs.Count);
            yield return new YieldOperation();
        }

        private IReadOnlyList<IteratorOutput> CalculateOutputs()
        {
            var outputs = new List<IteratorOutput>();

            foreach (var column in _columns)
            {
                switch (column)
                {
                    case ResultColumn.Star star:
                        if (star.Table.HasValue) { throw new NotImplementedException(); }

                        outputs.AddRange(_input.Outputs);
                        break;

                    case ResultColumn.Expression expr:
                        var (operations, type) = Compile(expr.Value);

                        var name = expr.Alias.Map<IteratorOutputName>(x => new IteratorOutputName.Constant(x), () => new IteratorOutputName.Expression(expr.Value));

                        outputs.Add(new IteratorOutput(name, type, operations));
                        break;

                    default: throw new ArgumentOutOfRangeException(nameof(column), $"Unhandled type: {column.GetType().FullName}");
                }
            }

            return outputs;
        }

        private (IReadOnlyCollection<IOperation>, ColumnType) Compile(Expression expr)
        {
            switch (expr)
            {
                case ColumnNameExpression column:
                    var result = _input.Outputs.Single(x => x.Name.Matches(column.Name));

                    return (result.LoadOperations, result.Type);

                default: throw new ArgumentOutOfRangeException(nameof(expr), $"Unhandled type: {expr.GetType().FullName}");
            }
        }
    }
}