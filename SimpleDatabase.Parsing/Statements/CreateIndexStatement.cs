using System.Collections.Generic;

namespace SimpleDatabase.Parsing.Statements;

public class CreateIndexStatement : StatementDataDefinition
{
    public string IndexName { get; }
    public string TableName { get; }
    public bool ExistsCheck { get; }
    public IReadOnlyList<IndexColumnDefinition> KeyColumns { get; }
    public IReadOnlyList<string> DataColumns { get; }

    public CreateIndexStatement(string indexName, string tableName, bool existsCheck, IReadOnlyList<IndexColumnDefinition> keyColumns, IReadOnlyList<string> dataColumns)
    {
        IndexName = indexName;
        TableName = tableName;
        ExistsCheck = existsCheck;
        KeyColumns = keyColumns;
        DataColumns = dataColumns;
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