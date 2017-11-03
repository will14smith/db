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
        public REPLTests()
        {
            _file = Path.GetTempFileName();
        }

        public void Dispose()
        {
            File.Delete(_file);
        }

        private readonly string _file;

        [Theory]
        [InlineData(new[]
        {
            ".exit"
        }, new[]
        {
            "db >"
        }, 0, ExitCode.Success)]
        [InlineData(new[]
        {
            ".unknown",
            ".exit"
        }, new[]
        {
            "db > Unrecognized command '.unknown'.",
            "db >"
        }, 0, ExitCode.Success)]
        [InlineData(new[]
        {
            "insert 0 a b",
            "select",
            ".exit"
        }, new[]
        {
            "db > Executed.",
            "db > (0, a, b)",
            "Executed.",
            "db >"
        }, 0, ExitCode.Success)]
        [InlineData(new[]
        {
            "unknown",
            ".exit"
        }, new[]
        {
            "db > Unrecognized keyword at start of 'unknown'.",
            "db >"
        }, 0, ExitCode.Success)]
        [InlineData(new[]
        {
            ".constants",
            ".exit"
        }, new[]
        {
            "db > Constants:",
            "RowSize: 291",
            "CommonNodeHeaderSize: 6",
            "LeafNodeHeaderSize: 10",
            "LeafNodeCellSize: 295",
            "LeafNodeSpaceForCells: 4086",
            "LeafNodeMaxCells: 13",
            "db >"
        }, 0, ExitCode.Success)]
        [InlineData(new[]
        {
            "insert 3 a b",
            "insert 1 a b",
            "insert 2 a b",
            ".btree",
            ".exit"
        }, new[]
        {
            "db > Executed.",
            "db > Executed.",
            "db > Executed.",
            "db > Tree:",
            "- leaf (size 3)",
            "  - 1",
            "  - 2",
            "  - 3",
            "db >"
        }, 0, ExitCode.Success)]
        [InlineData(new[]
        {
            "insert 1 a b",
            "insert 1 a b",
            "select",
            ".exit"
        }, new[]
        {
            "db > Executed.",
            "db > Error: Duplicate key.",
            "db > (1, a, b)",
            "Executed.",
            "db >"
        }, 0, ExitCode.Success)]
        public void RunningCommands_HasCorrectSnapshot(string[] commands, string[] expectedOutput, int outputOffset, ExitCode expectedCode)
        {
            var fakeOutput = new FakeREPLOutput();
            var fakeInput = new FakeREPLInput(commands);

            ExitCode code;
            using (var repl = new REPL(fakeInput, fakeOutput, _file))
            {
                code = repl.Run();
            }

            EqualWithDiff(expectedOutput, fakeOutput.Output.Split(Environment.NewLine).Skip(outputOffset).Select(x => x.TrimEnd()));
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
                Assert.True(false, output.ToString());
        }

        [Fact]
        public void PrintingInternalNodes()
        {
            var commands = new string[16];
            for (var i = 0; i < 14; i++)
                commands[i] = $"insert {i} user{i} person{i}@example.com";
            commands[14] = ".btree";
            commands[15] = ".exit";

            var outputs = new[]
            {
                "db > Tree:",
                "- internal (size 1)",
                "  - leaf (size 7)",
                "    - 0",
                "    - 1",
                "    - 2",
                "    - 3",
                "    - 4",
                "    - 5",
                "    - 6",
                "- key 6",
                "  - leaf (size 7)",
                "    - 7",
                "    - 8",
                "    - 9",
                "    - 10",
                "    - 11",
                "    - 12",
                "    - 13",
                "db >"
            };
            
            RunningCommands_HasCorrectSnapshot(commands, outputs, 14, ExitCode.Success);
        }
    }
}