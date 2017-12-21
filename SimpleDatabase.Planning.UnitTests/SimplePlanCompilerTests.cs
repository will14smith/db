using System.Collections.Generic;
using SimpleDatabase.Parsing.Expressions;
using SimpleDatabase.Parsing.Statements;
using SimpleDatabase.Planning.Nodes;
using SimpleDatabase.Schemas;
using SimpleDatabase.Schemas.Types;
using SimpleDatabase.Storage;
using SimpleDatabase.Utils;
using Xunit;
using Xunit.Abstractions;
using Table = SimpleDatabase.Schemas.Table;

namespace SimpleDatabase.Planning.UnitTests
{
    public class SimplePlanCompilerTests
    {
        private readonly ITestOutputHelper _output;

        public SimplePlanCompilerTests(ITestOutputHelper output)
        {
            this._output = output;
        }

        public static IReadOnlyCollection<object[]> Plans = new List<object[]>
        {
            new object[] {"Constant", new Plan(new ConstantNode(
                new [] { "a", "b", },
                new []
                {
                    new Expression[] { new NumberLiteralExpression(1), new StringLiteralExpression("a") },
                    new Expression[] { new NumberLiteralExpression(2), new StringLiteralExpression("b") },
                }
            )) },
            new object[] {"Scan", new Plan(new ScanTableNode("table")) },
            new object[] {"Project *", new Plan(new ProjectionNode(new ScanTableNode("table"), new []{ new ResultColumn.Star(Option.None<string>()) })) },
            new object[] {"Project column", new Plan(new ProjectionNode(new ScanTableNode("table"), new[]{ new ResultColumn.Expression(new ColumnNameExpression("name"), Option.None<string>()) })) },
            new object[] {"Filter name='a'", new Plan(new FilterNode(new ScanTableNode("table"), new BinaryExpression(BinaryOperator.Equal, new ColumnNameExpression("name"), new StringLiteralExpression("a")))) },
            new object[] {"Project & Filter", new Plan(
                new ProjectionNode(
                    new FilterNode(
                        new ScanTableNode("table"),
                        new BinaryExpression(BinaryOperator.Equal, new ColumnNameExpression("name"), new StringLiteralExpression("a"))
                    ),
                    new[] { new ResultColumn.Expression(new ColumnNameExpression("name"), Option.None<string>()) }
                ))
            },
            new object[] {"Filter & Project", new Plan(
                new FilterNode(
                    new ProjectionNode(
                        new ScanTableNode("table"),
                        new[] { new ResultColumn.Expression(new ColumnNameExpression("name"), Option.None<string>()) }
                    ),
                    new BinaryExpression(BinaryOperator.Equal, new ColumnNameExpression("name"), new StringLiteralExpression("a"))
                ))
            },
            new object[] {"Filter constant", new Plan(
                new FilterNode(
                    new ConstantNode(
                        new [] { "a", "b", },
                        new []
                        {
                            new Expression[] { new NumberLiteralExpression(1), new StringLiteralExpression("a") },
                            new Expression[] { new NumberLiteralExpression(2), new StringLiteralExpression("b") },
                        }
                    ),
                    new BinaryExpression(BinaryOperator.Equal, new ColumnNameExpression("b"), new StringLiteralExpression("a"))
                ))
            },
            new object[] {"Insert", new Plan(new InsertNode("table", new ScanTableNode("table"))) },
            new object[] {"Delete", new Plan(new DeleteNode(new ScanTableNode("table"))) },
            new object[] {"Delete & filter", new Plan(new DeleteNode(new FilterNode(new ScanTableNode("table"), new BinaryExpression(BinaryOperator.Equal, new ColumnNameExpression("name"), new StringLiteralExpression("a"))))) },
            new object[] {"Sort", new Plan(new SortNode(new ScanTableNode("table"), new [] { new OrderExpression(new ColumnNameExpression("name"), Order.Ascending) })) },
            new object[] {"Sort & filter", new Plan(new SortNode(new FilterNode(new ScanTableNode("table"), new BinaryExpression(BinaryOperator.Equal, new ColumnNameExpression("name"), new StringLiteralExpression("a"))), new [] { new OrderExpression(new ColumnNameExpression("name"), Order.Ascending) })) },
        };

        [Theory]
        [MemberData(nameof(Plans))]
        public void TestCanCompile(string name, Plan plan)
        {
            var database = new Database(new[]
            {
                new Table("table", new []{ new Column("id", new ColumnType.Integer()), new Column("name", new ColumnType.String(127)), new Column("email", new ColumnType.String(255)) }, new Index[0])
            });
            var compiler = new PlanCompiler(database);

            var program = compiler.Compile(plan);

            Assert.NotNull(program);

            _output.WriteLine(program.ToString());
        }
    }
}
