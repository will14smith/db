using System;
using System.IO;
using System.Linq;
using Xunit;

namespace SimpleDatabase.CLI.UnitTests
{
    public class REPLTests : IDisposable
    {
        private readonly string _file;

        public REPLTests()
        {
            _file = Path.GetTempFileName();
        }

        [Theory]
        [InlineData(new[]
        {
            ".exit"
        }, new[] {
            "db >"
        }, ExitCode.Success)]
        [InlineData(new[]
        {
            ".unknown",
            ".exit"
        }, new[] {
            "db > Unrecognized command '.unknown'.",
            "db >"
        }, ExitCode.Success)]
        [InlineData(new[]
        {
            "insert 0 a b",
            "select",
            ".exit"
        }, new[] {
            "db > Executed.",
            "db > (0, a, b)",
            "Executed.",
            "db >"
        }, ExitCode.Success)]
        [InlineData(new[]
        {
            "unknown",
            ".exit"
        }, new[] {
            "db > Unrecognized keyword at start of 'unknown'.",
            "db >"
        }, ExitCode.Success)]
        [InlineData(new[]
        {
            ".constants",
            ".exit"
        }, new[] {
            "db > Constants:",
            "RowSize: 291",
            "CommonNodeHeaderSize: 6",
            "LeafNodeHeaderSize: 10",
            "LeafNodeCellSize: 295",
            "LeafNodeSpaceForCells: 4086",
            "LeafNodeMaxCells: 13",
            "db >",
        }, ExitCode.Success)]
        public void RunningCommands_HasCorrectSnapshot(string[] commands, string[] expectedOutput, ExitCode expectedCode)
        {
            var fakeOutput = new FakeREPLOutput();
            var fakeInput = new FakeREPLInput(commands);
            var repl = new REPL(fakeInput, fakeOutput, _file);

            var code = repl.Run();

            Assert.Equal(expectedOutput, fakeOutput.Output.Split(Environment.NewLine).Select(x => x.TrimEnd()));
            Assert.Equal(expectedCode, code);
        }

        public void Dispose()
        {
            File.Delete(_file);
        }
    }
}
