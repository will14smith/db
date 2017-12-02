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
            "INSERT INTO table VALUES(0, 'a', 'b')",
            "SELECT * FROM table",
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
            "INSERT INTO table VALUES(3, 'a', 'b')",
            "INSERT INTO table VALUES(1, 'a', 'b')",
            "INSERT INTO table VALUES(2, 'a', 'b')",
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
            "INSERT INTO table VALUES(3, 'a', 'b')",
            "INSERT INTO table VALUES(1, 'a', 'b')",
            "INSERT INTO table VALUES(2, 'a', 'b')",
            "SELECT * FROM table",
            ".exit"
        }, new[]
        {
            "db > (1, a, b)",
            "(2, a, b)",
            "(3, a, b)",
            "Executed.",
            "db >"
        }, 3, ExitCode.Success)]
        [InlineData(new[]
        {
            "INSERT INTO table VALUES(1, 'a', 'b')",
            "INSERT INTO table VALUES(1, 'a', 'b')",
            "SELECT * FROM table",
            ".exit"
        }, new[]
        {
            "db > Executed.",
            "db > Error: Duplicate key.",
            "db > (1, a, b)",
            "Executed.",
            "db >"
        }, 0, ExitCode.Success)]
        public void RunningCommands_HasCorrectSnapshot(IReadOnlyList<string> commands, IReadOnlyCollection<string> expectedOutput, int outputOffset, ExitCode expectedCode)
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
            var commands = new string[17];
            for (var i = 0; i < 14; i++)
                commands[i] = $"INSERT INTO table VALUES({i}, 'user{i}', 'person{i}@example.com')";
            commands[14] = ".btree";
            commands[15] = "INSERT INTO table VALUES(14, 'user14', 'person14@example.com')";
            commands[16] = ".exit";

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
                "db > Executed.",
                "db >"
            };

            RunningCommands_HasCorrectSnapshot(commands, outputs, 14, ExitCode.Success);
        }

        [Fact]
        public void InsertingInternalNodesInReverse()
        {
            var commands = new string[50];
            var outputs = new List<string>();
            for (var i = 0; i < 48; i++)
            {
                commands[i] = $"INSERT INTO table VALUES ({47 - i}, 'user{47 - i}', 'person{47 - i}@example.com')";
                outputs.Add((i == 0 ? "db > " : "") + $"({i}, user{i}, person{i}@example.com)");
            }
            commands[48] = "SELECT * FROM table";
            commands[49] = ".exit";
            outputs.Add("Executed.");
            outputs.Add("db >");

            RunningCommands_HasCorrectSnapshot(commands, outputs, 48, ExitCode.Success);
        }

        [Fact]
        public void SelectingInternalNodes()
        {
            var commands = new string[16];
            var outputs = new List<string>();
            for (var i = 0; i < 14; i++)
            {
                commands[i] = $"INSERT INTO table VALUES({i}, 'user{i}', 'person{i}@example.com')";
                outputs.Add((i == 0 ? "db > " : "") + $"({i}, user{i}, person{i}@example.com)");
            }
            commands[14] = "SELECT * FROM table";
            commands[15] = ".exit";
            outputs.Add("Executed.");
            outputs.Add("db >");

            RunningCommands_HasCorrectSnapshot(commands, outputs, 14, ExitCode.Success);
        }

        [Fact]
        public void SplittingInternalNodes()
        {
            var commands = new string[7000];
            var outputs = new List<string>();
            for (var i = 0; i < 6998; i++)
            {
                commands[i] = $"INSERT INTO table VALUES({i}, 'user{i}', 'person{i}@example.com')";
                outputs.Add((i == 0 ? "db > " : "") + $"({i}, user{i}, person{i}@example.com)");
            }
            commands[6998] = "SELECT * FROM table";
            commands[6999] = ".exit";
            outputs.Add("Executed.");
            outputs.Add("db >");

            RunningCommands_HasCorrectSnapshot(commands, outputs, 6998, ExitCode.Success);
        }

        [Fact]
        public void DeleteRootLeaf()
        {
            var commands = new string[6];
            for (var i = 0; i < 3; i++)
                commands[i] = $"INSERT INTO table VALUES({i}, 'user{i}', 'person{i}@example.com')";
            commands[3] = "delete 1";
            commands[4] = ".btree";
            commands[5] = ".exit";

            var outputs = new[]
            {
                "db > Executed.",
                "db > Tree:",
                "- leaf (size 2)",
                "  - 0",
                "  - 2",
                "db >"
            };

            RunningCommands_HasCorrectSnapshot(commands, outputs, 3, ExitCode.Success);
        }
        [Fact]
        public void DeleteRootLeaf_Single()
        {
            var commands = new string[4];
            commands[0] = "INSERT INTO table VALUES(0, 'user0', 'person0@example.com')";
            commands[1] = "delete 0";
            commands[2] = ".btree";
            commands[3] = ".exit";

            var outputs = new[]
            {
                "db > Executed.",
                "db > Tree:",
                "- leaf (size 0)",
                "db >"
            };

            RunningCommands_HasCorrectSnapshot(commands, outputs, 1, ExitCode.Success);
        }
        [Fact]
        public void DeleteInternalNodes_BorrowFromNext()
        {
            var commands = new string[18];
            for (var i = 0; i < 14; i++)
                commands[i] = $"INSERT INTO table VALUES({i}, 'user{i}', 'person{i}@example.com')";
            commands[14] = "delete 1";
            commands[15] = "delete 2";
            commands[16] = ".btree";
            commands[17] = ".exit";

            var outputs = new[]
            {
                "db > Tree:",
                "- internal (size 1)",
                "  - leaf (size 6)",
                "    - 0",
                "    - 3",
                "    - 4",
                "    - 5",
                "    - 6",
                "    - 7",
                "- key 7",
                "  - leaf (size 6)",
                "    - 8",
                "    - 9",
                "    - 10",
                "    - 11",
                "    - 12",
                "    - 13",
                "db >"
            };

            RunningCommands_HasCorrectSnapshot(commands, outputs, 16, ExitCode.Success);
        }
        [Fact]
        public void DeleteInternalNodes_BorrowFromPrev()
        {
            var commands = new string[18];
            for (var i = 0; i < 14; i++)
                commands[i] = $"INSERT INTO table VALUES({i}, 'user{i}', 'person{i}@example.com')";
            commands[14] = "delete 7";
            commands[15] = "delete 8";
            commands[16] = ".btree";
            commands[17] = ".exit";

            var outputs = new[]
            {
                "db > Tree:",
                "- internal (size 1)",
                "  - leaf (size 6)",
                "    - 0",
                "    - 1",
                "    - 2",
                "    - 3",
                "    - 4",
                "    - 5",
                "- key 5",
                "  - leaf (size 6)",
                "    - 6",
                "    - 9",
                "    - 10",
                "    - 11",
                "    - 12",
                "    - 13",
                "db >"
            };

            RunningCommands_HasCorrectSnapshot(commands, outputs, 16, ExitCode.Success);
        }
        [Fact]
        public void DeleteInternalNodes_MergeWithNext()
        {
            var commands = new List<string>();
            for (var i = 0; i < 14; i++)
                commands[i] = $"INSERT INTO table VALUES({i}, 'user{i}', 'person{i}@example.com')";
            commands.Add("delete 7");
            commands.Add("delete 1");
            commands.Add("delete 2");
            commands.Add(".btree");
            commands.Add(".exit");

            var outputs = new[]
            {
                "db > Tree:",
                "- leaf (size 11)",
                "  - 0",
                "  - 3",
                "  - 4",
                "  - 5",
                "  - 6",
                "  - 8",
                "  - 9",
                "  - 10",
                "  - 11",
                "  - 12",
                "  - 13",
                "db >"
            };

            RunningCommands_HasCorrectSnapshot(commands, outputs, 17, ExitCode.Success);
        }
        [Fact]
        public void DeleteInternalNodes_MergeWithPrev()
        {
            var commands = new List<string>();
            for (var i = 0; i < 14; i++)
                commands[i] = $"INSERT INTO table VALUES({i}, 'user{i}', 'person{i}@example.com')";
            commands.Add("delete 1");
            commands.Add("delete 7");
            commands.Add("delete 8");
            commands.Add(".btree");
            commands.Add(".exit");

            var outputs = new[]
            {
                "db > Tree:",
                "- leaf (size 11)",
                "  - 0",
                "  - 2",
                "  - 3",
                "  - 4",
                "  - 5",
                "  - 6",
                "  - 9",
                "  - 10",
                "  - 11",
                "  - 12",
                "  - 13",
                "db >"
            };

            RunningCommands_HasCorrectSnapshot(commands, outputs, 17, ExitCode.Success);
        }

    }
}