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

            CompilePredicate(loopStart, _predicate);
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        private void CompilePredicate(ProgramLabel falseTarget, Expression expression)
        {
            var innerOutput = (IteratorOutput.Row)_input.Output;

            switch (expression)
            {
                case BinaryExpression binary:
                    switch (binary.Operator)
                    {
                        case BinaryOperator.Equal: 
                            CompileExpr(innerOutput, binary.Left).Load(_generator);
                            CompileExpr(innerOutput, binary.Right).Load(_generator);
                            _generator.Emit(new ConditionalJumpOperation(Comparison.NotEqual, falseTarget)); 
                            break;
                        case BinaryOperator.NotEqual: 
                            CompileExpr(innerOutput, binary.Left).Load(_generator);
                            CompileExpr(innerOutput, binary.Right).Load(_generator);
                            _generator.Emit(new ConditionalJumpOperation(Comparison.Equal, falseTarget)); 
                            break;
                        
                        case BinaryOperator.BooleanAnd:
                            CompilePredicate(falseTarget, binary.Left);
                            CompilePredicate(falseTarget, binary.Right);
                            
                            break;
                        
                        case BinaryOperator.BooleanOr:
                            // TODO this could be more efficient by getting the left predicate to jump to the true label and falling through to the false
                            var leftFalseTarget = _generator.NewLabel("or_lhs_false");
                            var leftTrueTarget = _generator.NewLabel("or_lhs_true");
                            CompilePredicate(leftFalseTarget, binary.Left);
                            _generator.Emit(new JumpOperation(leftTrueTarget));
                            
                            _generator.MarkLabel(leftFalseTarget);
                            CompilePredicate(falseTarget, binary.Right);

                            _generator.MarkLabel(leftTrueTarget);
                            break;
                        
                        default: throw new ArgumentOutOfRangeException(nameof(binary.Operator), $"Unhandled type: {binary.Operator}");
                    }

                    break;

                default: throw new ArgumentOutOfRangeException(nameof(expression), $"Unhandled type: {expression.GetType().FullName}");
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