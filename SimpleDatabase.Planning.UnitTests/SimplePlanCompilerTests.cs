using System.Collections.Generic;
using SimpleDatabase.Parsing.Expressions;
using SimpleDatabase.Parsing.Statements;
using SimpleDatabase.Planning.Nodes;
using SimpleDatabase.Schemas;
using SimpleDatabase.Schemas.Types;
using SimpleDatabase.Utils;
using Xunit;
using Table = SimpleDatabase.Schemas.Table;

namespace SimpleDatabase.Planning.UnitTests
{
    public class SimplePlanCompilerTests
    {
        public static IReadOnlyCollection<object[]> Plans = new List<object[]>
        {
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
                )) },
            new object[] {"Filter & Project", new Plan(
                new FilterNode(
                    new ProjectionNode(
                        new ScanTableNode("table"),
                        new[] { new ResultColumn.Expression(new ColumnNameExpression("name"), Option.None<string>()) }
                    ),
                    new BinaryExpression(BinaryOperator.Equal, new ColumnNameExpression("name"), new StringLiteralExpression("a"))
                ))
            }
        };

        [Theory]
        [MemberData(nameof(Plans))]
        public void TestCanCompile(string name, Plan plan)
        {
            var database = new Database(new Dictionary<string, Table>
            {
                { "table", new Table("table", new []{ new Column("id", new ColumnType.Integer()), new Column("name", new ColumnType.String(127)), new Column("email", new ColumnType.String(255)) }) }
            }, new Dictionary<string, int>
            {
                { "table", 0 },
            });
            var compiler = new PlanCompiler(database);

            var program = compiler.Compile(plan);

            Assert.NotNull(program);
        }
    }
}
