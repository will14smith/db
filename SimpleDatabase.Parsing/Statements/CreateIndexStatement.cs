using System.Collections.Generic;

namespace SimpleDatabase.Parsing.Statements;

public class CreateIndexStatement : StatementDataDefinition
{
    public string IndexName { get; }
    public string TableName { get; }
    public bool ExistsCheck { get; }
    public IReadOnlyList<IndexColumnDefinition> Columns { get; }

    public CreateIndexStatement(string indexName, string tableName, bool existsCheck, IReadOnlyList<IndexColumnDefinition> columns)
    {
        IndexName = indexName;
        TableName = tableName;
        ExistsCheck = existsCheck;
        Columns = columns;
    }
}

public class IndexColumnDefinition
{
    public string Name { get; }
    public Order Order { get; }

    public IndexColumnDefinition(string name, Order order)
    {
        Name = name;
        Order = order;
    }
}