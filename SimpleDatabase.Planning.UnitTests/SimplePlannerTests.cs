using System.Collections.Generic;
using SimpleDatabase.Parsing.Expressions;
using SimpleDatabase.Parsing.Statements;
using SimpleDatabase.Planning.Nodes;
using SimpleDatabase.Schemas;
using SimpleDatabase.Schemas.Types;
using SimpleDatabase.Storage;
using SimpleDatabase.Utils;
using Xunit;
using Table = SimpleDatabase.Schemas.Table;

namespace SimpleDatabase.Planning.UnitTests
{
    public class SimplePlannerTests
    {
        private readonly Database _database = new Database(new Table[]
        {
            new Table("table",
                new []
                {
                    new Column("id", new ColumnType.Integer()),
                    new Column("name", new ColumnType.String(255)),
                    new Column("email", new ColumnType.String(1023))
                },
                new []
                {
                    new Index("k_email", new KeyStructure(new [] { (new Column("email", new ColumnType.String(1023)), KeyOrdering.Ascending) }, new Column[0])),
                }),
        });

        [Fact]
        public void Test_Star_NoFilter()
        {
            var statement = new SelectStatement(
                new List<ResultColumn> { new ResultColumn.Star(Option.None<string>()) },
                new Parsing.Statements.Table.TableName("table"),
                Option.None<Expression>(),
                new OrderExpression[0]
            );
            var planner = new Planner(_database);

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
                new Parsing.Statements.Table.TableName("table"),
                Option.None<Expression>(),
                new OrderExpression[0]
            );
            var planner = new Planner(_database);

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
                new Parsing.Statements.Table.TableName("table"),
                Option.Some<Expression>(new BinaryExpression(BinaryOperator.Equal, new ColumnNameExpression("name"), new StringLiteralExpression("a"))),
                new OrderExpression[0]
            );
            var planner = new Planner(_database);

            var plan = planner.Plan(statement);

            var project = Assert.IsType<ProjectionNode>(plan.RootNode);
            Assert.Single(project.Columns, x => x is ResultColumn.Star);
            var filter = Assert.IsType<FilterNode>(project.Input);
            Assert.IsType<BinaryExpression>(filter.Predicate);
            var scan = Assert.IsType<ScanTableNode>(filter.Input);
            Assert.Equal("table", scan.TableName);
        }

        [Fact]
        public void Test_Star_OrderBy()
        {
            var statement = new SelectStatement(
                new List<ResultColumn> { new ResultColumn.Star(Option.None<string>()) },
                new Parsing.Statements.Table.TableName("table"),
                Option.None<Expression>(),
                new[] { new OrderExpression(new ColumnNameExpression("name"), Order.Ascending) }
            );
            var planner = new Planner(_database);

            var plan = planner.Plan(statement);

            var sort = Assert.IsType<SortNode>(plan.RootNode);
            Assert.Single(sort.Orderings, x => x.Expression is ColumnNameExpression);
            var project = Assert.IsType<ProjectionNode>(sort.Input);
            Assert.Single(project.Columns, x => x is ResultColumn.Star);
            var scan = Assert.IsType<ScanTableNode>(project.Input);
            Assert.Equal("table", scan.TableName);
        }

        [Fact]
        public void Test_Star_OrderBy_Using_Index()
        {
            var statement = new SelectStatement(
                new List<ResultColumn> { new ResultColumn.Star(Option.None<string>()) },
                new Parsing.Statements.Table.TableName("table"),
                Option.None<Expression>(),
                new[] { new OrderExpression(new ColumnNameExpression("email"), Order.Ascending) }
            );
            var planner = new Planner(_database);

            var plan = planner.Plan(statement);

            var project = Assert.IsType<ProjectionNode>(plan.RootNode);
            Assert.Single(project.Columns, x => x is ResultColumn.Star);
            var scan = Assert.IsType<ScanIndexNode>(project.Input);
            Assert.Equal("table", scan.TableName);
            Assert.Equal("k_email", scan.IndexName);
        }

        [Fact]
        public void Test_Insert()
        {
            var statement = new InsertStatement(
                "table",
                new[] { "a", "b" },
                new[]
                {
                    new []{ new NumberLiteralExpression(1), new NumberLiteralExpression(2) },
                    new []{ new NumberLiteralExpression(2), new NumberLiteralExpression(3) },
                });
            var planner = new Planner(_database);

            var plan = planner.Plan(statement);

            var insert = Assert.IsType<InsertNode>(plan.RootNode);
            Assert.Equal(statement.Table, insert.TableName);
            var constant = Assert.IsType<ConstantNode>(insert.Input);
            Assert.Equal(statement.Columns, constant.Columns);
            Assert.Equal(statement.Values, constant.Values);
        }

        [Fact]
        public void Test_Delete()
        {
            var statement = new DeleteStatement(
                "table",
                Option.Some<Expression>(new BinaryExpression(BinaryOperator.Equal, new ColumnNameExpression("a"), new StringLiteralExpression("a")))
                );
            var planner = new Planner(_database);

            var plan = planner.Plan(statement);

            var delete = Assert.IsType<DeleteNode>(plan.RootNode);
            var filter = Assert.IsType<FilterNode>(delete.Input);
            Assert.IsType<BinaryExpression>(filter.Predicate);
            var scan = Assert.IsType<ScanTableNode>(filter.Input);
            Assert.Equal(statement.Table, scan.TableName);
        }
    }
}
