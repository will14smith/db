using Xunit;

namespace SimpleDatabase.Core.UnitTests
{
    public class TableTests
    {
        [Fact]
        public void Insert_OneRow_ShouldReturnRowNumber0()
        {
            var row = CreateRow();
            var table = new Table();

            var result = table.Insert(new InsertStatement(row));

            var success = Assert.IsType<InsertResult.Success>(result);
            Assert.Equal(0, success.RowNumber);
        }
        [Fact]
        public void Insert_SecondRow_ShouldReturnRowNumber1()
        {
            var row = CreateRow();
            var table = new Table();
            table.Insert(new InsertStatement(row));

            var result = table.Insert(new InsertStatement(row));

            var success = Assert.IsType<InsertResult.Success>(result);
            Assert.Equal(1, success.RowNumber);

        }
        [Fact]
        public void Insert_MaxRows_ShouldReturnTableFull()
        {
            var row = CreateRow();
            var table = new Table();
            for (var i = 0; i < Table.TableMaxRows; i++)
            {
                table.Insert(new InsertStatement(row));
            }

            var result = table.Insert(new InsertStatement(row));

            Assert.IsType<InsertResult.TableFull>(result);
        }

        [Fact]
        public void InsertThenSelect_ShouldReturnInsertedRow()
        {
            var row = CreateRow();
            var table = new Table();
            table.Insert(new InsertStatement(row));

            var result = table.Select(new SelectStatement());

            var success = Assert.IsType<SelectResult.Success>(result);
            Assert.Equal(1, success.Rows.Count);
        }
        [Fact]
        public void InsertsThenSelect_ShouldReturnAllRows()
        {
            var row = CreateRow();
            var table = new Table();
            for (var i = 0; i < Table.TableMaxRows; i++)
            {
                table.Insert(new InsertStatement(row));
            }

            var result = table.Select(new SelectStatement());

            var success = Assert.IsType<SelectResult.Success>(result);
            Assert.Equal(Table.TableMaxRows, success.Rows.Count);
        }

        private static Row CreateRow()
        {
            return new Row(1, "a", "b");
        }
    }
}
