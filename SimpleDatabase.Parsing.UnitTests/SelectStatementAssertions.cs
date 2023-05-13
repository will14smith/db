using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Primitives;
using SimpleDatabase.Parsing.Expressions;
using SimpleDatabase.Parsing.Statements;
using SimpleDatabase.Utils;

namespace SimpleDatabase.Parsing.UnitTests
{
    public class SelectStatementAssertions : ReferenceTypeAssertions<SelectStatement, SelectStatementAssertions>
    {
        public SelectStatementAssertions(SelectStatement subject) : base(subject) { }

        protected override string Identifier => "select_statement";

        public AndConstraint<SelectStatementAssertions> HaveColumns(IEnumerable<ResultColumn> expected, string because = "", params object[] becauseArgs)
        {
            Subject.Columns.Should().ContainInOrder(expected, because, becauseArgs);
            
            return new AndConstraint<SelectStatementAssertions>(this);
        }

        public AndConstraint<SelectStatementAssertions> HaveTable(TableRef expected, string because = "", params object[] becauseArgs)
        {
            Subject.Table.Should().Be(expected, because, becauseArgs);
            
            return new AndConstraint<SelectStatementAssertions>(this);
        }
        
        public AndConstraint<SelectStatementAssertions> HaveWhere(Expression expected, string because = "", params object[] becauseArgs)
        {
            Subject.Where.Should().Be(Option.Some(expected), because, becauseArgs);
            
            return new AndConstraint<SelectStatementAssertions>(this);
        }      
        public AndConstraint<SelectStatementAssertions> NotHaveWhere(string because = "", params object[] becauseArgs)
        {
            Subject.Where.Should().Be(Option.None<Expression>(), because, becauseArgs);
            
            return new AndConstraint<SelectStatementAssertions>(this);
        }
        
        public AndConstraint<SelectStatementAssertions> HaveOrdering(IEnumerable<OrderExpression> expected, string because = "", params object[] becauseArgs)
        {
            Subject.Ordering.Should().ContainInOrder(expected, because, becauseArgs);
            
            return new AndConstraint<SelectStatementAssertions>(this);
        }
        public AndConstraint<SelectStatementAssertions> NotHaveOrdering(string because = "", params object[] becauseArgs)
        {
            Subject.Ordering.Should().BeEmpty(because, becauseArgs);
            
            return new AndConstraint<SelectStatementAssertions>(this);
        }
    }
}