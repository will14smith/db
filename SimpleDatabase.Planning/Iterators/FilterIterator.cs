using System;
using System.Collections.Generic;
using System.Linq;
using SimpleDatabase.Execution;
using SimpleDatabase.Execution.Operations;
using SimpleDatabase.Execution.Operations.Constants;
using SimpleDatabase.Execution.Operations.Jumps;
using SimpleDatabase.Parsing.Expressions;
using SimpleDatabase.Schemas.Types;

namespace SimpleDatabase.Planning.Iterators
{
    public class FilterIterator : IIterator
    {
        private readonly IIterator _input;
        private readonly Expression _predicate;

        private readonly ProgramLabel _moveNext;
        private readonly ProgramLabel _checkPredicate;
        private readonly ProgramLabel _moveNextExit;

        public FilterIterator(IIterator input, Expression predicate)
        {
            _input = input;
            _predicate = predicate;

            _moveNext = ProgramLabel.Create();
            _checkPredicate = ProgramLabel.Create();
            _moveNextExit = ProgramLabel.Create();
        }

        public IReadOnlyDictionary<SlotLabel, SlotDefinition> Slots => _input.Slots;
        public IReadOnlyList<IteratorOutput> Outputs => _input.Outputs;

        public IEnumerable<IOperation> Init(ProgramLabel emptyTarget)
        {
            var inputInit = _input.Init(emptyTarget);

            return inputInit.Concat(new IOperation[]
            {
                new JumpOperation(_checkPredicate),
            });
        }

        public IEnumerable<IOperation> MoveNext(ProgramLabel loopStartTarget)
        {
            var inputMoveNext = _input.MoveNext(_checkPredicate);

            // S: input.MoveNext(P)                 # try move to the next row, if we have one then go to P
            //    JMP E                             # there wasn't a next row so go to E
            // P: JMP loopStartTarget if pred==true # check the predicate, if it matches go to loopStartTarget
            //    JMP S                             # the predicate didn't match so go to S
            // E: ...                               # exit the move next 

            return new IOperation[] { _moveNext }
                .Concat(inputMoveNext)
                .Concat(new IOperation[]
                {
                    new JumpOperation(_moveNextExit),
                    _checkPredicate,
                })
                .Concat(Compile(loopStartTarget))
                .Concat(new IOperation[]
                {
                    new JumpOperation(_moveNext),
                    _moveNextExit
                });
        }

        private IEnumerable<IOperation> Compile(ProgramLabel trueTarget)
        {
            var output = new List<IOperation>();

            switch (_predicate)
            {
                case BinaryExpression binary:
                    var (left, _) = CompileExpr(binary.Left);
                    var (right, _) = CompileExpr(binary.Right);

                    output.AddRange(left);
                    output.AddRange(right);


                    switch (binary.Operator)
                    {
                        case BinaryOperator.Equal: output.Add(new ConditionalJumpOperation(Comparison.Equal, trueTarget)); break;
                        case BinaryOperator.NotEqual: output.Add(new ConditionalJumpOperation(Comparison.NotEqual, trueTarget)); break;
                        default: throw new ArgumentOutOfRangeException(nameof(binary.Operator), $"Unhandled type: {binary.Operator}");
                    }

                    return output;

                default: throw new ArgumentOutOfRangeException(nameof(_predicate), $"Unhandled type: {_predicate.GetType().FullName}");
            }
        }

        // TODO same as Compile in Projection iterator?
        private (IReadOnlyCollection<IOperation>, ColumnType) CompileExpr(Expression expr)
        {
            switch (expr)
            {
                case ColumnNameExpression column:
                    var result = _input.Outputs.Single(x => x.Name.Matches(column.Name));

                    return (result.LoadOperations, result.Type);

                case StringLiteralExpression str:
                    return (new [] { new ConstStringOperation(str.Value) }, new ColumnType.String(str.Value.Length));


                default: throw new ArgumentOutOfRangeException(nameof(expr), $"Unhandled type: {expr.GetType().FullName}");
            }
        }
    }
}