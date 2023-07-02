using System;
using System.Collections.Generic;
using SimpleDatabase.Parsing.Expressions;

namespace SimpleDatabase.Parsing.Tables;

public class TableFrom
{
    public TableAlias Table { get; }
    public IReadOnlyList<TableJoin> Joins { get; }

    public TableFrom(TableAlias table) : this(table, Array.Empty<TableJoin>()) { }
    public TableFrom(TableAlias table, IReadOnlyList<TableJoin> joins)
    {
        Table = table;
        Joins = joins;
    }
}

public class TableJoin
{
    public TableAlias Table { get; }
    public Expression? Predicate { get; }

    public TableJoin(TableAlias table, Expression? predicate)
    {
        Table = table;
        Predicate = predicate;
    }
}