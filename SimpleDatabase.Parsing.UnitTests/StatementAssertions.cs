using FluentAssertions;
using FluentAssertions.Primitives;
using SimpleDatabase.Parsing.Statements;

namespace SimpleDatabase.Parsing.UnitTests
{
    public class StatementAssertions : ReferenceTypeAssertions<Statement, StatementAssertions>
    {
        public StatementAssertions(Statement subject) : base(subject) { }

        protected override string Identifier => "statement";

        public AndWhichConstraint<StatementAssertions, SelectStatement> BeSelect(string because = "", params object[] becauseArgs)
        {
            return Subject.Should().BeOfType<SelectStatement>(because, becauseArgs);
        }
    }
}