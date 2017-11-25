using SimpleDatabase.Parsing.Antlr;
using SimpleDatabase.Parsing.Expressions;

namespace SimpleDatabase.Parsing.Visitors
{
    public class ExpressionVisitor : SQLBaseVisitor<Expression>
    {
        public override Expression VisitExpression(SQLParser.ExpressionContext context)
        {
            return base.VisitExpression(context);
        }
    }
}