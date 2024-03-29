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
            var folderName = "_test" + DateTime.Now.Ticks;
            _folder = Path.Combine(Path.GetTempPath(), folderName);
            Directory.CreateDirectory(_folder);
        }

        public void Dispose()
        {
            Directory.Delete(_folder, true);
        }

        private readonly string _folder;

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
            "INSERT INTO tbl VALUES(0, 'a', 'b')",
            "SELECT * FROM tbl",
            ".exit"
        }, new[]
        {
            "db > Executed.",
            "db > (0, a, b)",
            "Executed.",
            "db >"
        }, 0, ExitCode.Success)]
        // Skip: error output of parser currently isn't very good
        // [InlineData(new[]
        // {
        //     "unknown",
        //     ".exit"
        // }, new[]
        // {
        //     "db > Unrecognized keyword at start of 'unknown'.",
        //     "db >"
        // }, 0, ExitCode.Success)]
        [InlineData(new[]
        {
            "INSERT INTO tbl VALUES(3, 'a', 'b3')",
            "INSERT INTO tbl VALUES(1, 'a', 'b1')",
            "INSERT INTO tbl VALUES(2, 'a', 'b2')",
            "SELECT * FROM tbl ORDER BY id",
            ".exit"
        }, new[]
        {
            "db > (1, a, b1)",
            "(2, a, b2)",
            "(3, a, b3)",
            "Executed.",
            "db >"
        }, 3, ExitCode.Success)]
        // Skip: error handling isn't very good currently 
        // [InlineData(new[]
        // {
        //     "INSERT INTO tbl VALUES(1, 'a', 'b')",
        //     "INSERT INTO tbl VALUES(1, 'a', 'b')",
        //     "SELECT * FROM tbl",
        //     ".exit"
        // }, new[]
        // {
        //     "db > Executed.",
        //     "db > Error: Duplicate key.",
        //     "db > (1, a, b)",
        //     "Executed.",
        //     "db >"
        // }, 0, ExitCode.Success)]
        public void RunningCommands_HasCorrectSnapshot(IReadOnlyList<string> commands, IReadOnlyCollection<string> expectedOutput, int outputOffset, ExitCode expectedCode)
        {
            var fakeOutput = new FakeREPLOutput();
            var fakeInput = new FakeREPLInput(commands);

            Seed.EnsureExists(_folder);
            
            ExitCode code;
            using (var repl = new REPL(fakeInput, fakeOutput, _folder))
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
        public void InsertingInternalNodesInReverse()
        {
            var commands = new string[50];
            var outputs = new List<string>();
            for (var i = 0; i < 48; i++)
            {
                var x = 47 - i;

                commands[i] = $"INSERT INTO tbl VALUES ({x}, 'user{x}', 'person{x}@example.com')";
                outputs.Add((i == 0 ? "db > " : "") + $"({x}, user{x}, person{x}@example.com)");
            }
            commands[48] = "SELECT * FROM tbl";
            commands[49] = ".exit";
            outputs.Add("Executed.");
            outputs.Add("db >");

            RunningCommands_HasCorrectSnapshot(commands, outputs, 48, ExitCode.Success);
        }

        [Fact]
        public void SelectingAcrossMultiplePages()
        {
            var commands = new string[7000];
            var outputs = new List<string>();
            for (var i = 0; i < 6998; i++)
            {
                commands[i] = $"INSERT INTO tbl VALUES({i}, 'user{i}', 'person{i}@example.com')";
                outputs.Add((i == 0 ? "db > " : "") + $"({i}, user{i}, person{i}@example.com)");
            }
            commands[6998] = "SELECT * FROM tbl";
            commands[6999] = ".exit";
            outputs.Add("Executed.");
            outputs.Add("db >");

            RunningCommands_HasCorrectSnapshot(commands, outputs, 6998, ExitCode.Success);
        }

        [Fact(Skip = "failing to find the next to delete in HeapCursor.Delete")]
        public void DeleteAllItems()
        {
            var commands = new string[4];
            commands[0] = "INSERT INTO tbl VALUES(0, 'user0', 'person0@example.com')";
            commands[1] = "DELETE FROM tbl";
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
        public void Sorting()
        {
            var commands = new List<string>
            {
                "INSERT INTO tbl VALUES (1, 'c', 'c@c.c')",
                "INSERT INTO tbl VALUES (2, 'a', 'a@a.a')",
                "INSERT INTO tbl VALUES (3, 'b', 'b1@b.b')",
                "INSERT INTO tbl VALUES (4, 'a', 'b2@b.b')",

                "SELECT * FROM tbl ORDER BY name, email DESC",

                ".exit"
            };

            var outputs = new List<string>
            {
                "db > (4, a, b2@b.b)",
                "(2, a, a@a.a)",
                "(3, b, b1@b.b)",
                "(1, c, c@c.c)",
                "Executed.",
                "db >"
            };

            RunningCommands_HasCorrectSnapshot(commands, outputs, 4, ExitCode.Success);
        }
        [Fact]
        public void SortingUsingIndex()
        {
            var commands = new List<string>
            {
                "INSERT INTO tbl VALUES (1, 'c', 'c@c.c')",
                "INSERT INTO tbl VALUES (2, 'a', 'a@a.a')",
                "INSERT INTO tbl VALUES (3, 'b', 'b1@b.b')",
                "INSERT INTO tbl VALUES (4, 'a', 'b2@b.b')",

                "SELECT * FROM tbl ORDER BY email ASC",

                ".exit"
            };

            var outputs = new List<string>
            {
                "db > (2, a, a@a.a)",
                "(3, b, b1@b.b)",
                "(4, a, b2@b.b)",
                "(1, c, c@c.c)",
                "Executed.",
                "db >"
            };

            RunningCommands_HasCorrectSnapshot(commands, outputs, 4, ExitCode.Success);
        }

        [Fact]
        public void CommittedInsert_ShouldAppear()
        {
            var commands = new List<string>
            {
                ".begin",
                "INSERT INTO tbl VALUES (1, 'c', 'c@c.c')",
                ".commit",

                "SELECT * FROM tbl ORDER BY name, email DESC",

                ".exit"
            };

            var outputs = new List<string>
            {
                "db > Beginning transaction",
                "db > Executed.",
                "db > Committing transaction",
                "db > (1, c, c@c.c)",
                "Executed.",
                "db >"
            };

            RunningCommands_HasCorrectSnapshot(commands, outputs, 0, ExitCode.Success);
        }
        [Fact]
        public void InsertInCurrentTx_ShouldAppear()
        {
            var commands = new List<string>
            {
                ".begin",
                "INSERT INTO tbl VALUES (1, 'c', 'c@c.c')",
                "SELECT * FROM tbl ORDER BY name, email DESC",
                ".abort",

                ".exit"
            };

            var outputs = new List<string>
            {
                "db > Beginning transaction",
                "db > Executed.",
                "db > (1, c, c@c.c)",
                "Executed.",
                "db > Aborting transaction",
                "db >"
            };

            RunningCommands_HasCorrectSnapshot(commands, outputs, 0, ExitCode.Success);
        }
        [Fact]
        public void AbortedInsert_ShouldNotAppear()
        {
            var commands = new List<string>
            {
                ".begin",
                "INSERT INTO tbl VALUES (1, 'c', 'c@c.c')",
                ".abort",

                "SELECT * FROM tbl ORDER BY name, email DESC",

                ".exit"
            };

            var outputs = new List<string>
            {
                "db > Beginning transaction",
                "db > Executed.",
                "db > Aborting transaction",
                "db > Executed.",
                "db >"
            };

            RunningCommands_HasCorrectSnapshot(commands, outputs, 0, ExitCode.Success);
        }
    }
}