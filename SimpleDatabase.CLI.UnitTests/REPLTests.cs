using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
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
        [InlineData(new[]
        {
            "insert 3 a b",
            "insert 1 a b",
            "insert 2 a b",
            ".btree",
            ".exit"
        }, new[] {
            "db > Executed.",
            "db > Executed.",
            "db > Executed.",
            "db > Tree:",
            "leaf (size 3)",
            "  - 0 : 3",
            "  - 1 : 1",
            "  - 2 : 2",
            "db >",
        }, ExitCode.Success)]
        public void RunningCommands_HasCorrectSnapshot(string[] commands, string[] expectedOutput, ExitCode expectedCode)
        {
            var fakeOutput = new FakeREPLOutput();
            var fakeInput = new FakeREPLInput(commands);
            var repl = new REPL(fakeInput, fakeOutput, _file);

            var code = repl.Run();

            EqualWithDiff(expectedOutput, fakeOutput.Output.Split(Environment.NewLine).Select(x => x.TrimEnd()));
            Assert.Equal(expectedCode, code);
        }

        private void EqualWithDiff(IEnumerable<string> expected, IEnumerable<string> actual)
        {
            var diffBuilder = new InlineDiffBuilder(new Differ());
            var diff = diffBuilder.BuildDiffModel(string.Join(Environment.NewLine, expected), string.Join(Environment.NewLine, actual));

            var output = new StringBuilder();

            foreach (var line in diff.Lines)
            {
                switch (line.Type)
                {
                    case ChangeType.Inserted:
                        output.Append("+ ");
                        break;
                    case ChangeType.Deleted:
                        output.Append("- ");
                        break;
                    default:
                        output.Append("  ");
                        break;
                }

                output.AppendLine(line.Text);
            }

            if (diff.Lines.Any(x => x.Type != ChangeType.Unchanged))
            {
                Assert.True(false, output.ToString());
            }
        }

        public void Dispose()
        {
            File.Delete(_file);
        }
    }
}
