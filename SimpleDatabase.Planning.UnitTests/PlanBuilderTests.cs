using System;
using SimpleDatabase.Parsing.Expressions;
using SimpleDatabase.Parsing.Statements;
using SimpleDatabase.Planning.Nodes;
using SimpleDatabase.Schemas;
using SimpleDatabase.Schemas.Types;
using Xunit;

namespace SimpleDatabase.Planning.UnitTests;

public class PlanBuilderTests
{
    [Fact]
    public void T()
    {
        var tableIndex = new TableIndex("ix_name",
            new KeyStructure(new [] { (new Column("name", new ColumnType.String(10)), KeyOrdering.Ascending) }, Array.Empty<Column>()));
        var table = new Table("tbl", new[]
        {
            new Column("id", new ColumnType.Integer()),
            new Column("name", new ColumnType.String(10)),
            new Column("country", new ColumnType.String(10)),
        }, new[] { tableIndex });

        var predicate = new BinaryExpression(BinaryOperator.Equal, new ColumnNameExpression("name"), new StringLiteralExpression("Will"));

        PlanBuilder.EnumeratePlans(
            table,
            new []
            {
                new ResultColumn.Star(null),
            },
            predicate,
            Array.Empty<OrderExpression>()
        );
    }

    [Fact]
    public void X()
    {
        // CREATE TABLE tbl (id int, name char(31), country char(2));
        // CREATE INDEX ix_tbl_name ON tbl (name);
        // EXPLAIN SELECT * FROM tbl WHERE name = 'Will'; 
        
        var tableIndex = new TableIndex("ix_name",
            new KeyStructure(new [] { (new Column("name", new ColumnType.String(10)), KeyOrdering.Ascending) }, Array.Empty<Column>()));

        var indexScan = new SeekIndexNode("i0", "tbl", tableIndex, new BinaryExpression(BinaryOperator.Equal, new ColumnNameExpression("name"), new StringLiteralExpression("Will")));
        var rowIdLookup = new RowIdLookupNode("t1", "tbl", new NodeOutputExpression(indexScan.Alias, 0));
        
        var nestedLoopJoin = new NestedLoopJoinNode("j2", indexScan, rowIdLookup, null);
    }
}