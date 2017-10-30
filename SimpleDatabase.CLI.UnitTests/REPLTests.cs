using System;
using Xunit;

namespace SimpleDatabase.CLI.UnitTests
{
    public class REPLTests
    {
        [Theory]
        [InlineData(new[] { ".exit" }, "db >", ExitCode.Success)]
        [InlineData(new[] { ".unknown", ".exit" }, "db > Unrecognized command '.unknown'.\ndb >", ExitCode.Success)]
        [InlineData(new[] { "insert 0 a b", "select", ".exit" }, "db > Executed.\ndb > (0, a, b)\nExecuted.\ndb >", ExitCode.Success)]
        [InlineData(new[] { "unknown", ".exit" }, "db > Unrecognized keyword at start of 'unknown'.\ndb >", ExitCode.Success)]
        public void RunningCommands_HasCorrectSnapshot(string[] commands, string expectedOutput, ExitCode expectedCode)
        {
            var fakeOutput = new FakeREPLOutput();
            var fakeInput = new FakeREPLInput(commands);
            var repl = new REPL(fakeInput, fakeOutput);

            var code = repl.Run();

            Assert.Equal(expectedOutput.Replace("\n", Environment.NewLine), fakeOutput.Output.TrimEnd());
            Assert.Equal(expectedCode, code);
        }
    }
}
