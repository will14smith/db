using System;
using System.Linq;
using Xunit;

namespace SimpleDatabase.Core.UnitTests
{
    public partial class RowTests
    {
        [Fact]
        public void ToString_ReturnsTupleFormatString()
        {
            var row = new Row(0, "a", "b");

            Assert.Equal("(0, a, b)", row.ToString());
        }

        private static string CreateString(int size)
        {
            return string.Join("", Enumerable.Range(0, size).Select(x => "a"));
        }
    }
}
