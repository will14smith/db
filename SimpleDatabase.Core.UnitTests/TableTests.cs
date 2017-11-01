using System.Collections.Generic;
using System.Linq;
using Moq;
using SimpleDatabase.Core.Paging;
using Xunit;

namespace SimpleDatabase.Core.UnitTests
{
    public class TableTests
    {
        [Fact]
        public void Insert_OneRow_ShouldReturnRowNumber0()
        {
            var row = CreateRow();
            var (table, _) = CreateTable();

            var result = table.Insert(new InsertStatement(row));

            var success = Assert.IsType<InsertResult.Success>(result);
            Assert.Equal(0, success.RowNumber);
        }
        [Fact]
        public void Insert_SecondRow_ShouldReturnRowNumber1()
        {
            var row = CreateRow();
            var (table, _) = CreateTable();
            table.Insert(new InsertStatement(row));

            var result = table.Insert(new InsertStatement(row));

            var success = Assert.IsType<InsertResult.Success>(result);
            Assert.Equal(1, success.RowNumber);

        }
        [Fact]
        public void Insert_MaxRows_ShouldReturnTableFull()
        {
            var row = CreateRow();
            var (table, _) = CreateTable();
            for (var i = 0; i < Pager.MaxRows; i++)
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
            var (table, _) = CreateTable();
            table.Insert(new InsertStatement(row));

            var result = table.Select(new SelectStatement());

            var success = Assert.IsType<SelectResult.Success>(result);
            Assert.Equal(1, success.Rows.Count);
            Assert.Equal(row, success.Rows.First(), new ToStringComparer<Row>());
        }
        [Fact]
        public void InsertsThenSelect_ShouldReturnAllRows()
        {
            var row = CreateRow();
            var (table, _) = CreateTable();
            for (var i = 0; i < Pager.MaxRows; i++)
            {
                table.Insert(new InsertStatement(row));
            }

            var result = table.Select(new SelectStatement());

            var success = Assert.IsType<SelectResult.Success>(result);
            Assert.Equal(Pager.MaxRows, success.Rows.Count);
        }

        [Fact]
        public void Dispose_WithSingleRow_ShouldFlush()
        {
            var row = CreateRow();
            var (table, pager) = CreateTable();
            table.Insert(new InsertStatement(row));
            
            table.Dispose();

            pager.Verify(x => x.Flush(0, Row.RowSize), Times.Once);
        }
        [Fact]
        public void Dispose_WithMultiplePages_ShouldFlushAllPages()
        {
            var row = CreateRow();
            var (table, pager) = CreateTable();
            for (var i = 0; i < Pager.RowsPerPage + 1; i++)
            {
                table.Insert(new InsertStatement(row));
            }

            table.Dispose();

            pager.Verify(x => x.Flush(0, Pager.PageSize), Times.Once);
            pager.Verify(x => x.Flush(1, Row.RowSize), Times.Once);
        }

        private static (Table, Mock<IPager>) CreateTable()
        {
            var pager = new Mock<IPager>();
            pager.Setup(x => x.Get(It.IsAny<int>())).Returns(new Page(new byte[Pager.PageSize]));

            var table = new Table(pager.Object);

            return (table, pager);
        }

        private static Row CreateRow()
        {
            return new Row(1, "a", "b");
        }
    }

    public class ToStringComparer<T> : IEqualityComparer<T>
    {
        public bool Equals(T x, T y)
        {
            if (ReferenceEquals(x, null)) return ReferenceEquals(y, null);
            if (ReferenceEquals(y, null)) return false;

            return x.ToString() == y.ToString();
        }

        public int GetHashCode(T obj)
        {
            return obj?.ToString().GetHashCode() ?? 0;
        }
    }
}
