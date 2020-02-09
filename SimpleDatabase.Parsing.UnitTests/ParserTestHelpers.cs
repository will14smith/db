using FluentAssertions;
using SimpleDatabase.Parsing.Statements;

namespace SimpleDatabase.Parsing.UnitTests
{
    public static class ParserTestHelpers
    {
        public static Statement ParseStatement(string input)
        {
            var statements = Parser.Parse(input);

            return statements.Should()
                .ContainSingle("Expected a single statement")
                .Subject;
        }

        public static SelectStatement ParseSelectStatement(string input) =>
            ParseStatement(input).Should().BeSelect().Which;

        public static StatementAssertions Should(this Statement statement)
        {
            return new StatementAssertions(statement);
        }    
        public static SelectStatementAssertions Should(this SelectStatement statement)
        {
            return new SelectStatementAssertions(statement);
        }

    }
}