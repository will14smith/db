using System;
using System.Linq;
using SimpleDatabase.Parsing.Statements;
using SimpleDatabase.Schemas;
using SimpleDatabase.Schemas.Types;

namespace SimpleDatabase.Execution.Definitions;

public partial class DefinitionExecutor
{
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
        
        static Table ToTable(CreateTableStatement statement) =>
            new(
                statement.TableName,
                statement.Columns.Select(ToColumn).ToList(),
                Array.Empty<TableIndex>()
            );
        static Column ToColumn(ColumnDefinition def) => new(def.Name, ToColumnType(def.Type));
        static ColumnType ToColumnType(ColumnDefinitionType def) =>
            def.Name.ToLowerInvariant() switch
            {
                "int" => new ColumnType.Integer(),
                "char" => new ColumnType.String(def.Arguments[0])
            };
    }
}