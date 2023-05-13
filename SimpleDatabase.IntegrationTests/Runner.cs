using System.IO.Abstractions.TestingHelpers;
using System.Text;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using SimpleDatabase.Execution;
using SimpleDatabase.Execution.Definitions;
using SimpleDatabase.Execution.Transactions;
using SimpleDatabase.Parsing;
using SimpleDatabase.Parsing.Statements;
using SimpleDatabase.Planning;
using SimpleDatabase.Storage;
using SimpleDatabase.Storage.Paging;

namespace SimpleDatabase.IntegrationTests;

public class Runner
{
    public static IEnumerable<object[]> TestCases => LoadTestCases();

    private static IEnumerable<object[]> LoadTestCases()
    {
        var path = "Specs";
        var files = Directory.GetFiles(path);

        foreach (var test in files.GroupBy(Path.GetFileNameWithoutExtension))
        {
            var input = File.ReadAllText(Path.Combine(path, $"{test.Key}.in"));
            var outPath = Path.Combine(path, $"{test.Key}.out");
            var output = File.Exists(outPath) ? File.ReadAllText(outPath) : null;

            yield return new object[] { new TestCase(test.Key!, input, output) };
        }
    }

    [Theory]
    [MemberData(nameof(TestCases))]
    public void Run(TestCase testCase)
    {
        var output = new StringBuilder();
        
        var folder = "test";
        var fs = new MockFileSystem();
        fs.Directory.CreateDirectory(folder);

        using var pager = new Pager(new FolderPageSourceFactory(fs, folder));

        using var databaseManager = new DatabaseManager(pager);
        databaseManager.EnsureInitialised();

        var transactionManager = new TransactionManager();
        
        var statements = Parser.Parse(testCase.Input);

        foreach (var statement in statements)
        {
            switch (statement)
            {
                case StatementDataDefinition ddl: RunDdlStatement(ddl); break;
                case StatementDataManipulation dml: RunDmlStatement(dml); break;
                
                default: throw new ArgumentOutOfRangeException(nameof(statement));
            }
        }

        if (testCase.Output == null)
        {
            Assert.False(true, output.ToString());
        }
        else
        {
            AssertEqualWithDiff(testCase.Output, output.ToString());
        }

        void RunDdlStatement(StatementDataDefinition statement)
        {
            var executor = new DefinitionExecutor(databaseManager);
            
            var result = executor.Execute(statement);
            switch (result)
            {
                case DefinitionResult.Success success:
                    output.AppendLine(success.Message);
                    break;
                case DefinitionResult.Failure failure:
                    output.AppendLine($"error: {failure.Message}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(result));
            }
        }

        void RunDmlStatement(StatementDataManipulation statement)
        {
            var database = new Database(databaseManager.GetAllTables());

            var planner = new Planner(database);
            var plan = planner.Plan(statement);

            var compiler = new PlanCompiler(database);
            var program = compiler.Compile(plan);

            var transaction = transactionManager.Begin();

            var executor = new ProgramExecutor(program, databaseManager, transactionManager);
            var results = executor.Execute().ToList();
            var resultsAsString = results.Select(row => (IReadOnlyList<string>)row.Select(col => col.ToString() ?? string.Empty).ToList()).ToList();
            var columnSizes = FindColumnSizes(resultsAsString);
            
            foreach (var row in resultsAsString)
            {
                for (var i = 0; i < row.Count; i++)
                {
                    output.Append(" " + row[i].PadRight(columnSizes[i]) + " ");
                    if (i + 1 < row.Count)
                    {
                        output.Append('|');
                    }
                }

                output.AppendLine();
            }
            
            transaction.Commit();
        }
    }

    private static IReadOnlyList<int> FindColumnSizes(IEnumerable<IReadOnlyList<string>> rows)
    {
        List<int>? sizes = null;

        foreach (var row in rows)
        {
            if (sizes == null)
            {
                sizes = row.Select(x => x.Length).ToList();
            }
            else
            {
                for (var i = 0; i < row.Count; i++)
                {
                    sizes[i] = Math.Max(sizes[i], row[i].Length);
                }
            }
        }
        
        return (IReadOnlyList<int>?)sizes ?? Array.Empty<int>();
    }

    private void AssertEqualWithDiff(string expected, string actual)
    {
        var diffBuilder = new InlineDiffBuilder(new Differ());
        var diff = diffBuilder.BuildDiffModel(actual, expected);

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

}

public class TestCase
{
    public string Name { get; }
    public string Input { get; }
    public string? Output { get; }

    public TestCase(string name, string input, string? output)
    {
        Name = name;
        Input = input;
        Output = output;
    }

    public override string ToString() => Name;
}