using System;
using System.Linq;
using SimpleDatabase.Execution;
using SimpleDatabase.Execution.Operations.Jumps;
using SimpleDatabase.Parsing.Expressions;
using SimpleDatabase.Planning.Items;

namespace SimpleDatabase.Planning.Iterators
{
    public class FilterIterator : IIterator
    {
        private readonly IOperationGenerator _generator;
        private readonly IIterator _input;
        private readonly Expression _predicate;

        private readonly ProgramLabel _moveNext;
        private readonly ProgramLabel _checkPredicate;
        private readonly ProgramLabel _moveNextExit;

        public FilterIterator(IOperationGenerator generator, IIterator input, Expression predicate)
        {
            _generator = generator;
            _input = input;
            _predicate = predicate;

            _moveNext = generator.NewLabel("move next");
            _checkPredicate = generator.NewLabel("predicate");
            _moveNextExit = generator.NewLabel("done");

            Output = input.Output;
        }

        public IteratorOutput Output { get; }

        public void GenerateInit(ProgramLabel emptyTarget)
        {
            _input.GenerateInit(emptyTarget);
            _generator.Emit(new JumpOperation(_checkPredicate));
        }

        public void GenerateMoveNext(ProgramLabel loopStartTarget)
        {
            // S: input.MoveNext(P)                 # try move to the next row, if we have one then go to P
            //    JMP E                             # there wasn't a next row so go to E
            // P: JMP loopStartTarget if pred==true # check the predicate, if it matches go to loopStartTarget
            //    JMP S                             # the predicate didn't match so go to S
            // E: ...                               # exit the move next 

            _generator.MarkLabel(_moveNext);
            _input.GenerateMoveNext(_checkPredicate);
            _generator.Emit(new JumpOperation(_moveNextExit));

            _generator.MarkLabel(_checkPredicate);
            CompilePredicate(loopStartTarget);
            _generator.Emit(new JumpOperation(_moveNext));
            _generator.MarkLabel(_moveNextExit);
        }

        private void CompilePredicate(ProgramLabel trueTarget)
        {
            var innerOutput = (IteratorOutput.Row)_input.Output;

            switch (_predicate)
            {
                case BinaryExpression binary:
                    CompileExpr(innerOutput, binary.Left).Load(_generator);
                    CompileExpr(innerOutput, binary.Right).Load(_generator);

                    switch (binary.Operator)
                    {
                        case BinaryOperator.Equal: _generator.Emit(new ConditionalJumpOperation(Comparison.Equal, trueTarget)); break;
                        case BinaryOperator.NotEqual: _generator.Emit(new ConditionalJumpOperation(Comparison.NotEqual, trueTarget)); break;
                        default: throw new ArgumentOutOfRangeException(nameof(binary.Operator), $"Unhandled type: {binary.Operator}");
                    }

                    break;

                default: throw new ArgumentOutOfRangeException(nameof(_predicate), $"Unhandled type: {_predicate.GetType().FullName}");
            }
        }

        private Item CompileExpr(IteratorOutput.Row input, Expression expr)
        {
            switch (expr)
            {
                case ColumnNameExpression column:
                    var result = input.Columns.Single(x => x.Name.Matches(column.Name));

                    return result.Value;

                case StringLiteralExpression str:
                    return new ConstItem(str.Value);

                default: throw new ArgumentOutOfRangeException(nameof(expr), $"Unhandled type: {expr.GetType().FullName}");
            }
        }
    }
}