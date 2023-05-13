using FluentAssertions;
using Xunit;

namespace SimpleDatabase.Parsing.UnitTests
{
    public class ParserTests
    {
        [Theory]

        [InlineData("SELECT * FROM tbl")]
        [InlineData("SELECT tbl.* FROM tbl")]
    
        [InlineData("SELECT column FROM tbl")]
        [InlineData("SELECT column1, column2 FROM tbl")]

        [InlineData("SELECT * FROM tbl WHERE column = 1")]
        [InlineData("SELECT * FROM tbl WHERE column = 'abc'")]

        [InlineData("INSERT INTO tbl VALUES (1, 2)")]
        [InlineData("INSERT INTO tbl (a, b) VALUES ('a', 'b')")]

        [InlineData("DELETE FROM tbl")]
        [InlineData("DELETE FROM tbl WHERE column = 'a'")]
        
        public void CanParse(string input)
        {
            var statements = Parser.Parse(input);

            statements.Should().ContainSingle();
        }
    }
}
