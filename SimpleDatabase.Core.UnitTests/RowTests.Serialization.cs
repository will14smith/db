using System;
using Xunit;

namespace SimpleDatabase.Core.UnitTests
{
    public partial class RowTests
    {
        [Fact]
        public void SerializationRoundTrip_Id()
        {
            var offset = 100;
            var buffer = new byte[offset + Row.RowSize];
            var originalRow = new Row(123, "", "");
            var span = new Span<byte>(buffer, offset);

            originalRow.Serialize(span);
            var resultRow = Row.Deserialize(span);

            Assert.Equal(originalRow.Id, resultRow.Id);
        }

        [Fact]
        public void SerializationRoundTrip_Username_Empty()
        {
            var offset = 100;
            var buffer = new byte[offset + Row.RowSize];
            var originalRow = new Row(int.MaxValue, "", "abc");
            var span = new Span<byte>(buffer, offset);

            originalRow.Serialize(span);
            var resultRow = Row.Deserialize(span);

            Assert.Equal(originalRow.Username, resultRow.Username);
        }

        [Fact]
        public void SerializationRoundTrip_Username_Partial()
        {
            var offset = 100;
            var buffer = new byte[offset + Row.RowSize];
            var originalRow = new Row(int.MaxValue, CreateString(Row.UsernameSize - 1), "abc");
            var span = new Span<byte>(buffer, offset);

            originalRow.Serialize(span);
            var resultRow = Row.Deserialize(span);

            Assert.Equal(originalRow.Username, resultRow.Username);
        }

        [Fact]
        public void SerializationRoundTrip_Username_Full()
        {
            var offset = 100;
            var buffer = new byte[offset + Row.RowSize];
            var originalRow = new Row(int.MaxValue, CreateString(Row.UsernameSize), "abc");
            var span = new Span<byte>(buffer, offset);

            originalRow.Serialize(span);
            var resultRow = Row.Deserialize(span);

            Assert.Equal(originalRow.Username, resultRow.Username);
        }

        [Fact]
        public void SerializationRoundTrip_Email_Empty()
        {
            var offset = 100;
            var buffer = new byte[offset + Row.RowSize];
            var originalRow = new Row(int.MaxValue, "abc", "");
            var span = new Span<byte>(buffer, offset);

            originalRow.Serialize(span);
            var resultRow = Row.Deserialize(span);

            Assert.Equal(originalRow.Email, resultRow.Email);
        }

        [Fact]
        public void SerializationRoundTrip_Email_Partial()
        {
            var offset = 100;
            var buffer = new byte[offset + Row.RowSize];
            var originalRow = new Row(int.MaxValue, "abc", CreateString(Row.EmailSize - 1));
            var span = new Span<byte>(buffer, offset);

            originalRow.Serialize(span);
            var resultRow = Row.Deserialize(span);

            Assert.Equal(originalRow.Email, resultRow.Email);
        }

        [Fact]
        public void SerializationRoundTrip_Email_Full()
        {
            var offset = 100;
            var buffer = new byte[offset + Row.RowSize];
            var originalRow = new Row(int.MaxValue, "abc", CreateString(Row.EmailSize));
            var span = new Span<byte>(buffer, offset);

            originalRow.Serialize(span);
            var resultRow = Row.Deserialize(span);

            Assert.Equal(originalRow.Email, resultRow.Email);
        }
    }
}