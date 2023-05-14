using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using Antlr4.Runtime.Misc;
using SimpleDatabase.CLI.Commands;
using SimpleDatabase.Execution;
using SimpleDatabase.Execution.Definitions;
using SimpleDatabase.Execution.Transactions;
using SimpleDatabase.Parsing;
using SimpleDatabase.Parsing.Statements;
using SimpleDatabase.Planning;
using SimpleDatabase.Storage;
using SimpleDatabase.Storage.Paging;

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

            var pager = new Pager(new FolderPageSourceFactory(new FileSystem(), folder));
            var databaseManager = new DatabaseManager(pager);
            var txm = new TransactionManager();

            _state = new REPLState(
                pager,
                databaseManager,
                txm
            );
            
            _commands = new CommandHandler();
            _commands.Register("exit", new ExitCommand());
            _commands.Register("begin", new BeginTransactionCommand(_state, _output));
            _commands.Register("commit", new CommitTransactionCommand(_state, _output));
            _commands.Register("abort", new AbortTransactionCommand(_state, _output));
            _commands.Register("btree", new BTreeCommand(_state, _output));
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
            IReadOnlyCollection<Statement> statements;
            try
            {
                statements = Parser.Parse(input);
            }
            catch (ParseCanceledException ex)
            {
                return new ExecuteStatementResponse.SyntaxError(ex.Message);
            }
            
            foreach (var statement in statements)
            {
                switch (statement)
                {
                    case StatementDataDefinition ddl: RunDdlStatement(ddl); break;
                    case StatementDataManipulation dml: RunDmlStatement(dml); break;
                
                    default: throw new ArgumentOutOfRangeException(nameof(statement));
                }
            }

            _output.WriteLine("Executed.");

            return new ExecuteStatementResponse.Success();
            
            void RunDdlStatement(StatementDataDefinition statement)
            {
                var executor = new DefinitionExecutor(_state.DatabaseManager);
            
                executor.Execute(statement);
            }

            void RunDmlStatement(StatementDataManipulation statement)
            {
                var planner = new Planner(_state.Database);
                var plan = planner.Plan(statement);

                var compiler = new PlanCompiler(_state.Database);
                var program = compiler.Compile(plan);

                var executor = new ProgramExecutor(program, _state.DatabaseManager, _state.TransactionManager);
                foreach (var result in executor.Execute())
                {
                    _output.WriteLine("(" + string.Join(", ", result) + ")");
                }

                if (statement is ExplainStatement { Execute: true } explain)
                {
                    RunDmlStatement(explain.Statement);
                }
            }
        }

        public void Dispose()
        {
            _state.Transaction?.Dispose();
            _state.Pager.Dispose();
        }
    }
}
