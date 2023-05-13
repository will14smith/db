using System;
using System.Linq;
using SimpleDatabase.Parsing.Statements;
using SimpleDatabase.Schemas;
using SimpleDatabase.Schemas.Types;
using SimpleDatabase.Storage;

namespace SimpleDatabase.Execution.Definitions;

public class DefinitionExecutor
{
    private readonly DatabaseManager _databaseManager;

    public DefinitionExecutor(DatabaseManager databaseManager)
    {
        _databaseManager = databaseManager;
    }

    public DefinitionResult Execute(StatementDataDefinition statement)
    {
        switch (statement)
        {
            case CreateTableStatement createTable: return ExecuteCreateTable(createTable);

            default: throw new ArgumentOutOfRangeException(nameof(statement));
        }
    }

    private DefinitionResult ExecuteCreateTable(CreateTableStatement statement)
    {
        var existingTables = _databaseManager.GetAllTables();
        if (existingTables.Any(x => x.Name == statement.TableName))
        {
            return statement.ExistsCheck
                ? new DefinitionResult.Success("CREATE TABLE")
                : new DefinitionResult.Failure($"table '{statement.TableName}' already exists");
        }

        var table = ToTable(statement);
        
        var tableManager = _databaseManager.GetTableManagerFor(table);
        tableManager.EnsureInitialised();

        return new DefinitionResult.Success("CREATE TABLE");
    }

    private static Table ToTable(CreateTableStatement statement) =>
        new(
            statement.TableName,
            statement.Columns.Select(ToColumn).ToList(),
            Array.Empty<TableIndex>()
        );

    private static Column ToColumn(ColumnDefinition def) => new(def.Name, ToColumnType(def.Type));

    private static ColumnType ToColumnType(ColumnDefinitionType def) =>
        def.Name.ToLowerInvariant() switch
        {
            "int" => new ColumnType.Integer(),
            "char" => new ColumnType.String(def.Arguments[0])
        };
}

public abstract class DefinitionResult
{
    public class Success : DefinitionResult
    {
        public string Message { get; }

        public Success(string message)
        {
            Message = message;
        }
    }

    public class Failure : DefinitionResult
    {
        public string Message { get; }

        public Failure(string message)
        {
            Message = message;
        }
    }
}