using System.Collections.Generic;

namespace SimpleDatabase.Parsing.Statements;

public class CreateTableStatement : StatementDataDefinition
{
    public string TableName { get; }
    public bool ExistsCheck { get; }
    public IReadOnlyList<ColumnDefinition> Columns { get; }

    public CreateTableStatement(string tableName, bool existsCheck, IReadOnlyList<ColumnDefinition> columns)
    {
        TableName = tableName;
        ExistsCheck = existsCheck;
        Columns = columns;
    }
}

public class ColumnDefinition
{
    public string Name { get; }
    public ColumnDefinitionType Type { get; }

    public ColumnDefinition(string name, ColumnDefinitionType type)
    {
        Name = name;
        Type = type;
    }
}

public class ColumnDefinitionType
{
    public string Name { get; }
    public IReadOnlyList<int> Arguments { get; }

    public ColumnDefinitionType(string name, IReadOnlyList<int> arguments)
    {
        Name = name;
        Arguments = arguments;
    }
}