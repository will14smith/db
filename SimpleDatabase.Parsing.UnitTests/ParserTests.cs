using System;
using System.Collections.Generic;
using System.Text;
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
        [InlineData("SELECT * FROM table WHERE column = 'a'")]
        public void CanParse(string input)
        {
            var parser = new Parser();

            var statements = parser.Parse(input);

            Assert.Equal(1, statements.Count);
        }
    }
}
