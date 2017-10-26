using System;
using Xunit;

namespace SimpleDatabase.CLI.UnitTests
{
    public class REPLTests
    {
        [Theory]
        [InlineData(new[] { ".exit" }, "db > .exit", ExitCode.Success)]
        [InlineData(new[] { ".unknown", ".exit" }, "db > .unknown\nUnrecognized command '.unknown'.\ndb > .exit", ExitCode.Success)]
        public void RunningCommands_HasCorrectSnapshot(string[] commands, string expectedOutput, ExitCode expectedCode)
        {
            var fakeOutput = new FakeREPLOutput();
            var fakeInput = new FakeREPLInput(fakeOutput, commands);
            var repl = new REPL(fakeInput, fakeOutput);

            var code = repl.Run();

            Assert.Equal(expectedOutput.Replace("\n", Environment.NewLine), fakeOutput.Output.TrimEnd());
            Assert.Equal(expectedCode, code);
        }
    }
}
