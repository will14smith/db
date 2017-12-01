using System;
using System.Collections.Generic;
using SimpleDatabase.Execution;
using SimpleDatabase.Execution.Operations;
using SimpleDatabase.Parsing.Statements;

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

        private IReadOnlyList<IteratorOutput> CalculateOutputs()
        {
            var outputs = new List<IteratorOutput>();

            foreach (var column in _columns)
            {
                switch (column)
                {
                    case ResultColumn.Star star:
                        if(star.Table.HasValue) { throw new NotImplementedException(); }
                        
                        outputs.AddRange(_input.Outputs);
                        break;

                    default: throw new ArgumentOutOfRangeException(nameof(column), $"Unhandled type: {column.GetType().FullName}");
                }
            }

            return outputs;
        }
    }
}