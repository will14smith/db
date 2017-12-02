using System;
using System.Collections.Generic;
using System.Linq;
using SimpleDatabase.Execution;
using SimpleDatabase.Execution.Operations;
using SimpleDatabase.Execution.Operations.Jumps;
using SimpleDatabase.Parsing.Expressions;

namespace SimpleDatabase.Planning.Iterators
{
    public class ConstantIterator : IIterator
    {
        private readonly IReadOnlyList<string> _columns;
        private readonly IReadOnlyList<IReadOnlyList<Expression>> _values;
        
        public ConstantIterator(IReadOnlyList<string> columns, IReadOnlyList<IReadOnlyList<Expression>> values)
        {
            _columns = columns;
            _values = values;

            Functions = new Dictionary<FunctionLabel, Function>
            {
                { FunctionLabel.Create(), GenerateFunction() }
            };
        }

        public IReadOnlyDictionary<SlotLabel, SlotDefinition> Slots { get; }
        public IReadOnlyDictionary<FunctionLabel, Function> Functions { get;  }
        public IReadOnlyList<IteratorOutput> Outputs { get; }

        public IEnumerable<IOperation> Init(ProgramLabel emptyTarget)
        {
            // f(): yield _values[0]; yield _values[1]; ...; yield _values[N-1]

            // setup f

            // call co-routine done-addr

            if (_values.Any())
            {
                throw new NotImplementedException();
            }
            else
            {
                yield return new JumpOperation(emptyTarget);
            }
        }

        public IEnumerable<IOperation> MoveNext(ProgramLabel loopStartTarget)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IOperation> Yield()
        {
            throw new System.NotImplementedException();
        }

        private Function GenerateFunction()
        {
            throw new NotImplementedException();
        }
    }
}
