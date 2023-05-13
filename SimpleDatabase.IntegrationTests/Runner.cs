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
            var output = File.ReadAllText(Path.Combine(path, $"{test.Key}.out"));

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

        AssertEqualWithDiff(testCase.Output, output.ToString());

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
            foreach (var result in executor.Execute())
            {
                output.AppendLine("(" + string.Join(", ", result) + ")");
            }
            
            transaction.Commit();
        }
    }
    
    private void AssertEqualWithDiff(string expected, string actual)
    {
        var diffBuilder = new InlineDiffBuilder(new Differ());
        var diff = diffBuilder.BuildDiffModel(expected, actual);

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
    public string Output { get; }

    public TestCase(string name, string input, string output)
    {
        Name = name;
        Input = input;
        Output = output;
    }

    public override string ToString() => Name;
}