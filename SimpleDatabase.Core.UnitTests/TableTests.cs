﻿using System.Collections.Generic;
using System.Linq;
using Moq;
using SimpleDatabase.Core.Paging;
using SimpleDatabase.Core.Trees;
using Xunit;

namespace SimpleDatabase.Core.UnitTests
{
    public class TableTests
    {
        [Fact]
        public void Insert_OneRow_ShouldReturnId()
        {
            var row = CreateRow();
            var (table, _) = CreateTable();

            var result = table.Insert(new InsertStatement(row));

            var success = Assert.IsType<InsertResult.Success>(result);
            Assert.Equal(row.Id, success.Key);
        }
        [Fact]
        public void Insert_SecondRow_ShouldReturnId()
        {
            var (table, _) = CreateTable();
            table.Insert(new InsertStatement(CreateRow(1)));

            var result = table.Insert(new InsertStatement(CreateRow(2)));

            var success = Assert.IsType<InsertResult.Success>(result);
            Assert.Equal(2, success.Key);
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
            var (table, _) = CreateTable();
            for (var i = 0; i < NodeLayout.LeafNodeMaxCells; i++)
            {
                table.Insert(new InsertStatement(CreateRow(i)));
            }

            var result = table.Select(new SelectStatement());

            var success = Assert.IsType<SelectResult.Success>(result);
            Assert.Equal(NodeLayout.LeafNodeMaxCells, success.Rows.Count);
        }

        [Fact]
        public void Dispose_ShouldDisposePager()
        {
            var (table, pager) = CreateTable();
            
            table.Dispose();

            pager.Verify(x => x.Dispose(), Times.Once);
        }

        private static (Table, Mock<IPager>) CreateTable()
        {
            var pager = new Mock<IPager>();
            pager.Setup(x => x.Get(It.IsAny<int>())).Returns(new Page(0, new byte[Pager.PageSize]));

            var table = new Table(pager.Object);

            return (table, pager);
        }

        private static Row CreateRow(int id = 1)
        {
            return new Row(id, "a", "b");
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
