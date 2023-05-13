using System;
using System.Linq;
using SimpleDatabase.Parsing.Statements;
using SimpleDatabase.Schemas;

namespace SimpleDatabase.Execution.Definitions;

public partial class DefinitionExecutor
{
    private DefinitionResult ExecuteCreateIndex(CreateIndexStatement statement)
    {
        var existingTables = _databaseManager.GetAllTables();
        if (existingTables.Any(t => t.Indexes.Any(i => i.Name == statement.TableName)))
        {
            return statement.ExistsCheck
                ? new DefinitionResult.Success("CREATE INDEX")
                : new DefinitionResult.Failure($"index '{statement.IndexName}' already exists");
        }

        var tableManager = _databaseManager.GetTableManagerFor(statement.TableName);
        var currentTableSchema = tableManager.Table;
        
        var keyColumns = statement.Columns.Select(ToKey).ToList();
        var index = new TableIndex(statement.IndexName, new KeyStructure(keyColumns, Array.Empty<Column>()));
        
        var newTableSchema = new Table(currentTableSchema.Name, currentTableSchema.Columns, currentTableSchema.Indexes.Append(index).ToList());
        tableManager = _databaseManager.GetTableManagerFor(newTableSchema);
        tableManager.EnsureInitialised();

        return new DefinitionResult.Success("CREATE INDEX");
        
        (Column, KeyOrdering) ToKey(IndexColumnDefinition column) => (currentTableSchema.Columns.First(x => x.Name == column.Name), ToKeyOrder(column.Order));
        KeyOrdering ToKeyOrder(Order order) => order == Order.Ascending ? KeyOrdering.Ascending : KeyOrdering.Descending;
    }
}