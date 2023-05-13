using System;
using System.Linq;
using SimpleDatabase.Parsing.Statements;
using SimpleDatabase.Schemas;
using SimpleDatabase.Schemas.Types;
using SimpleDatabase.Storage;

namespace SimpleDatabase.Execution.Definitions;

public partial class DefinitionExecutor
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
            case CreateIndexStatement createIndex: return ExecuteCreateIndex(createIndex);

            default: throw new ArgumentOutOfRangeException(nameof(statement));
        }
    }
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