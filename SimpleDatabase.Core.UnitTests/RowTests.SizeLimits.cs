using System;
using Xunit;

namespace SimpleDatabase.Core.UnitTests
{
    public partial class RowTests
    {
        [Fact]
        public void Username_AtLengthLimit_ShouldBeAllowed()
        {
            var x = new Row(1, CreateString(Row.UsernameSize), "");
            Assert.NotNull(x);
        }

        [Fact]
        public void Username_OverLengthLimit_ShouldBeThrow()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Row(1, CreateString(Row.UsernameSize + 1), ""));
        }

        [Fact]
        public void Email_AtLengthLimit_ShouldBeAllowed()
        {
            var x = new Row(1, "", CreateString(Row.EmailSize));
            Assert.NotNull(x);
        }

        [Fact]
        public void Email_OverLengthLimit_ShouldBeThrow()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Row(1, "", CreateString(Row.EmailSize + 1)));
        }
    }
}