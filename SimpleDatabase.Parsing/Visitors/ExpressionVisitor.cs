using SimpleDatabase.Parsing.Antlr;
using SimpleDatabase.Parsing.Expressions;

namespace SimpleDatabase.Parsing.Visitors
{
    public class ExpressionVisitor : SQLBaseVisitor<Expression>
    {
        public override Expression VisitExpression_literal(SQLParser.Expression_literalContext context)
        {
            return context.literal_value().Accept(this);
        }
        public override Expression VisitLiteral_value(SQLParser.Literal_valueContext context)
        {
            var number = context.NUMBER_LITERAL();
            if (number != null)
            {
                return new NumberLiteralExpression(int.Parse(number.GetText()));
            }

            var str = context.STRING_LITERAL();
            if (str != null)
            {
                return new StringLiteralExpression(str.GetText());
            }

            return base.VisitLiteral_value(context);
        }

        public override Expression VisitExpression_column(SQLParser.Expression_columnContext context)
        {
            var name = context.column_name().GetText();

            return new ColumnNameExpression(name);
        }

        public override Expression VisitExpression_equality(SQLParser.Expression_equalityContext context)
        {
            var left = context.expression(0).Accept(this);
            var right = context.expression(1).Accept(this);

            var op = context.Operator.Text != "!=" ? BinaryOperator.Equal : BinaryOperator.NotEqual;

            return new BinaryExpression(op, left, right);
        }
    }
}