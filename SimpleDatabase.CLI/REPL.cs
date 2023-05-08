using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Antlr4.Runtime.Misc;
using SimpleDatabase.CLI.Commands;
using SimpleDatabase.Execution;
using SimpleDatabase.Execution.Transactions;
using SimpleDatabase.Parsing;
using SimpleDatabase.Parsing.Statements;
using SimpleDatabase.Planning;
using SimpleDatabase.Schemas;
using SimpleDatabase.Schemas.Types;
using SimpleDatabase.Storage;
using SimpleDatabase.Storage.Paging;
using Table = SimpleDatabase.Schemas.Table;

namespace SimpleDatabase.CLI
{
    public class REPL : IDisposable
    {
        private readonly IREPLInput _input;
        private readonly IREPLOutput _output;
        private readonly CommandHandler _commands;

        private readonly REPLState _state;
        
        public REPL(IREPLInput input, IREPLOutput output, string folder)
        {
            _input = input;
            _output = output;

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var storage = new FolderPageSourceFactory(folder);
            var pager = new Pager(storage);

            var table = CreateTable(pager, "table", new[]
            {
                new Column("id", new ColumnType.Integer()),
                new Column("name", new ColumnType.String(31)),
                new Column("email", new ColumnType.String(255)),
            }, new []
            {
                ("pk", new [] { ("id", KeyOrdering.Ascending) }),
                ("k_email", new [] { ("email", KeyOrdering.Ascending) }),
            });

            var database = new Database(new[] { table });

            var txm = new TransactionManager();

            _state = new REPLState(
                pager,
                table,
                database,
                txm
            );
            
            _commands = new CommandHandler();
            _commands.Register("exit", new ExitCommand());
            _commands.Register("begin", new BeginTransactionCommand(_state, _output));
            _commands.Register("commit", new CommitTransactionCommand(_state, _output));
            _commands.Register("abort", new AbortTransactionCommand(_state, _output));
            _commands.Register("btree", new BTreeCommand(_state, _output));
        }

        private Table CreateTable(IPager pager, string name, IReadOnlyList<Column> columns, IEnumerable<(string, (string, KeyOrdering)[])> indexDefs)
        {
            var indices = indexDefs.Select(x => new TableIndex(x.Item1, new KeyStructure(x.Item2.Select(c => (columns.Single(v => v.Name == c.Item1), c.Item2)).ToList(), new Column[0]))).ToList();

            var table = new Table(name, columns, indices);

            new TableCreator(pager).Create(table);

            return table;
        }

        public ExitCode Run()
        {
            while (true)
            {
                PrintPrompt();
                var line = ReadInput();

                if (line.StartsWith("."))
                {
                    var response = _commands.Handle(line);
                    switch (response)
                    {
                        case CommandResponse.Success _:
                            continue;
                        case CommandResponse.Exit exit:
                            return exit.Code;
                        case CommandResponse.Unrecognised resp:
                            _output.WriteLine("Unrecognized command '{0}'.", resp.Input);
                            continue;
                        default:
                            _output.WriteLine("Unrecognized meta command response '{0}'.", response.GetType().Name);
                            continue;
                    }
                }

                var statementResponse = _state.Transaction != null ? ExecuteStatement(_state.Transaction, line) : ExecuteStatement(line);

                switch (statementResponse)
                {
                    case ExecuteStatementResponse.Success _:
                        break;

                    case ExecuteStatementResponse.SyntaxError resp:
                        _output.WriteLine("Syntax error '{0}'.", resp.Error);
                        break;

                    case ExecuteStatementResponse.Unrecognised resp:
                        _output.WriteLine("Unrecognized keyword at start of '{0}'.", resp.Input);
                        break;

                    default:
                        _output.WriteLine("Unrecognized prepare statement response '{0}'.", statementResponse.GetType().Name);
                        continue;
                }

            }
        }

        private void PrintPrompt() => _output.Write("db > ");
        private string ReadInput() => _input.ReadLine() ?? string.Empty;

        private ExecuteStatementResponse ExecuteStatement(string input)
        {
            using var transaction = _state.TransactionManager.Begin();
            
            var result = ExecuteStatement(transaction, input);
            if (result is ExecuteStatementResponse.Success _)
            {
                transaction.Commit();
            }

            return result;
        }

        private ExecuteStatementResponse ExecuteStatement(ITransaction transaction, string input)
        {
            var planner = new Planner(_state.Database);
            var planCompiler = new PlanCompiler(_state.Database);

            IReadOnlyCollection<Statement> statements;
            try
            {
                statements = Parser.Parse(input);
            }
            catch (ParseCanceledException ex)
            {
                return new ExecuteStatementResponse.SyntaxError(ex.Message);
            }

            var plans = statements.Select(planner.Plan);
            var programs = plans.Select(planCompiler.Compile);

            foreach (var program in programs)
            {
                var executor = new ProgramExecutor(program, _state.Pager, _state.TransactionManager);

                foreach (var result in executor.Execute())
                {
                    _output.WriteLine("(" + string.Join(", ", result) + ")");
                }
            }

            _output.WriteLine("Executed.");

            return new ExecuteStatementResponse.Success();
        }

        public void Dispose()
        {
            _state.Transaction?.Dispose();
            _state.Pager.Dispose();
        }
    }
}
