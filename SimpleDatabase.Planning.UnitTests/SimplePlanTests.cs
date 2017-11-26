using System.Collections.Generic;
using SimpleDatabase.Parsing.Expressions;
using SimpleDatabase.Parsing.Statements;
using SimpleDatabase.Planning.Nodes;
using SimpleDatabase.Utils;
using Xunit;

namespace SimpleDatabase.Planning.UnitTests
{
    public class SimplePlanTests
    {
        [Fact]
        public void Test_Star_NoFilter()
        {
            var statement = new SelectStatement(
                new List<ResultColumn> { new ResultColumn.Star(Option.None<string>()) },
                new Table.TableName("table"),
                Option.None<Expression>()
            );
            var planner = new Planner();

            var plan = planner.Plan(statement);

            var project = Assert.IsType<ProjectionNode>(plan.RootNode);
            Assert.Single(project.Columns, x => x is ResultColumn.Star);
            var scan = Assert.IsType<ScanTableNode>(project.Input);
            Assert.Equal("table", scan.TableName);
        }

        [Fact]
        public void Test_ColumnList_NoFilter()
        {
            var statement = new SelectStatement(
                new List<ResultColumn>
                {
                    new ResultColumn.Expression(new ColumnNameExpression("name"), Option.None<string>()),
                    new ResultColumn.Expression(new ColumnNameExpression("email"), Option.None<string>()),
                },
                new Table.TableName("table"),
                Option.None<Expression>()
            );
            var planner = new Planner();

            var plan = planner.Plan(statement);

            var project = Assert.IsType<ProjectionNode>(plan.RootNode);
            Assert.Equal(2, project.Columns.Count);
            Assert.All(project.Columns, x => Assert.IsType<ResultColumn.Expression>(x));
            var scan = Assert.IsType<ScanTableNode>(project.Input);
            Assert.Equal("table", scan.TableName);
        }

        [Fact]
        public void Test_Star_Filter()
        {
            var statement = new SelectStatement(
                new List<ResultColumn> { new ResultColumn.Star(Option.None<string>()) },
                new Table.TableName("table"),
                Option.Some<Expression>(new BinaryExpression(BinaryOperator.Equal, new ColumnNameExpression("name"), new StringLiteralExpression("a")))
            );
            var planner = new Planner();

            var plan = planner.Plan(statement);

            var project = Assert.IsType<ProjectionNode>(plan.RootNode);
            Assert.Single(project.Columns, x => x is ResultColumn.Star);
            var filter = Assert.IsType<FilterNode>(project.Input);
            Assert.IsType<BinaryExpression>(filter.Predicate);
            var scan = Assert.IsType<ScanTableNode>(filter.Input);
            Assert.Equal("table", scan.TableName);
        }
    }
}
