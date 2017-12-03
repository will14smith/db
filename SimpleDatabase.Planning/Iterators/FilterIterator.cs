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


        public FilterIterator(IOperationGenerator generator, IIterator input, Expression predicate)
        {
            _generator = generator;
            _input = input;
            _predicate = predicate;

            Output = input.Output;
        }

        public IteratorOutput Output { get; }

        public void GenerateInit()
        {
            _input.GenerateInit();
        }

        public void GenerateMoveNext(ProgramLabel loopStart, ProgramLabel loopEnd)
        {
            _input.GenerateMoveNext(loopStart, loopEnd);

            CompilePredicate(loopStart);
        }

        private void CompilePredicate(ProgramLabel falseTarget)
        {
            var innerOutput = (IteratorOutput.Row)_input.Output;

            switch (_predicate)
            {
                case BinaryExpression binary:
                    CompileExpr(innerOutput, binary.Left).Load(_generator);
                    CompileExpr(innerOutput, binary.Right).Load(_generator);

                    switch (binary.Operator)
                    {
                        case BinaryOperator.Equal: _generator.Emit(new ConditionalJumpOperation(Comparison.NotEqual, falseTarget)); break;
                        case BinaryOperator.NotEqual: _generator.Emit(new ConditionalJumpOperation(Comparison.Equal, falseTarget)); break;
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

                case NumberLiteralExpression num:
                    return new ConstItem(num.Value);

                case StringLiteralExpression str:
                    return new ConstItem(str.Value);

                default: throw new ArgumentOutOfRangeException(nameof(expr), $"Unhandled type: {expr.GetType().FullName}");
            }
        }
    }
}