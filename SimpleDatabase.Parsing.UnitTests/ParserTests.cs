using FluentAssertions;
using Xunit;

namespace SimpleDatabase.Parsing.UnitTests
{
    public class ParserTests
    {
        [Theory]

        [InlineData("SELECT * FROM table")]
        [InlineData("SELECT table.* FROM table")]
    
        [InlineData("SELECT column FROM table")]
        [InlineData("SELECT column1, column2 FROM table")]

        [InlineData("SELECT * FROM table WHERE column = 1")]
        [InlineData("SELECT * FROM table WHERE column = 'abc'")]

        [InlineData("INSERT INTO table VALUES (1, 2)")]
        [InlineData("INSERT INTO table (a, b) VALUES ('a', 'b')")]

        [InlineData("DELETE FROM table")]
        [InlineData("DELETE FROM table WHERE column = 'a'")]
        
        public void CanParse(string input)
        {
            var statements = Parser.Parse(input);

            statements.Should().ContainSingle();
        }
    }
}
