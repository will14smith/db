using SimpleDatabase.Parsing.Expressions;
using SimpleDatabase.Parsing.Statements;
using SimpleDatabase.Utils;
using Xunit;
using static SimpleDatabase.Parsing.UnitTests.ParserTestHelpers; 

namespace SimpleDatabase.Parsing.UnitTests
{
    public class ParserTestsSelect
    {
        [Fact]
        public void SelectStarFromTable()
        {
            var statement = ParseSelectStatement("SELECT * FROM table");

            statement.Should()
                .HaveColumns(new [] { new ResultColumn.Star(null) })
                .And.HaveTable(new Table.TableName("table"))
                .And.NotHaveWhere()
                .And.NotHaveOrdering();
        }   
        
        [Fact]
        public void SelectColumn()
        {
            var statement = ParseSelectStatement("SELECT abc FROM table");

            statement.Should()
                .HaveColumns(new [] { new ResultColumn.Expression(new ColumnNameExpression("abc"), null),  });
        }  
        
        [Fact]
        public void SelectMultipleColumns()
        {
            var statement = ParseSelectStatement("SELECT abc, def FROM table");

            statement.Should()
                .HaveColumns(new []
                {
                    new ResultColumn.Expression(new ColumnNameExpression("abc"), null),
                    new ResultColumn.Expression(new ColumnNameExpression("def"), null),
                });
        }  
        
        [Fact]
        public void SelectColumnWithAlias()
        {
            var statement = ParseSelectStatement("SELECT abc as def, ghi FROM table");

            statement.Should()
                .HaveColumns(new []
                {
                    new ResultColumn.Expression(new ColumnNameExpression("abc"), "def"),
                    new ResultColumn.Expression(new ColumnNameExpression("ghi"), null),
                });
        }
        
        [Fact]
        public void WhereColumn()
        {
            var statement = ParseSelectStatement("SELECT * FROM table WHERE abc");

            statement.Should()
                .HaveWhere(new ColumnNameExpression("abc"));
        }    
        
        [Fact]
        public void WhereExpression()
        {
            var statement = ParseSelectStatement("SELECT * FROM table WHERE abc = 1");

            statement.Should()
                .HaveWhere(new BinaryExpression(BinaryOperator.Equal, new ColumnNameExpression("abc"), new NumberLiteralExpression(1)));
        }   
        
        [Fact]
        public void OrderByColumn()
        {
            var statement = ParseSelectStatement("SELECT * FROM table ORDER BY abc");

            statement.Should()
                .HaveOrdering(new []
                {
                    new OrderExpression(new ColumnNameExpression("abc"), Order.Ascending), 
                });
        }  
        
        [Fact]
        public void OrderByDescending()
        {
            var statement = ParseSelectStatement("SELECT * FROM table ORDER BY abc DESC");

            statement.Should()
                .HaveOrdering(new []
                {
                    new OrderExpression(new ColumnNameExpression("abc"), Order.Descending), 
                });        }    
        
        [Fact]
        public void OrderByMultiple()
        {
            var statement = ParseSelectStatement("SELECT * FROM table ORDER BY abc DESC, def");

            statement.Should()
                .HaveOrdering(new []
                {
                    new OrderExpression(new ColumnNameExpression("abc"), Order.Descending), 
                    new OrderExpression(new ColumnNameExpression("def"), Order.Ascending), 
                });
        }
    }
}