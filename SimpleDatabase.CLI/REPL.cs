using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;
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

        private readonly Pager _pager;
        private readonly Table _table;
        private readonly Database _database;

        private readonly TransactionManager _txm;
        private ITransaction _tx;

        public REPL(IREPLInput input, IREPLOutput output, string folder)
        {
            _input = input;
            _output = output;

            var storage = new FolderPageSourceFactory(folder);
            _pager = new Pager(storage);

            _table = CreateTable("table", new[]
            {
                new Column("id", new ColumnType.Integer()),
                new Column("name", new ColumnType.String(31)),
                new Column("email", new ColumnType.String(255)),
            }, new []
            {
                ("pk", new [] { ("id", KeyOrdering.Ascending) }),
                ("k_email", new [] { ("email", KeyOrdering.Ascending) }),
            });

            _database = new Database(new[] { _table });

            _txm = new TransactionManager();
        }

        private Table CreateTable(string name, IReadOnlyList<Column> columns, IEnumerable<(string, (string, KeyOrdering)[])> indexDefs)
        {
            var indices = indexDefs.Select(x => new Index(x.Item1, new KeyStructure(x.Item2.Select(c => (columns.Single(v => v.Name == c.Item1), c.Item2)).ToList(), new Column[0]))).ToList();

            var table = new Table(name, columns, indices);

            new TableCreator(_pager).Create(table);

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
                    var response = HandleMetaCommand(line);
                    switch (response)
                    {
                        case MetaCommandResponse.Success _:
                            continue;
                        case MetaCommandResponse.Exit exit:
                            return exit.Code;
                        case MetaCommandResponse.Unrecognised resp:
                            _output.WriteLine("Unrecognized command '{0}'.", resp.Input);
                            continue;
                        default:
                            _output.WriteLine("Unrecognized meta command response '{0}'.", response.GetType().Name);
                            continue;
                    }
                }

                var statementResponse = _tx != null ? ExecuteStatement(_tx, line) : ExecuteStatement(line);

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

        private void PrintPrompt()
        {
            _output.Write("db > ");
        }

        private string ReadInput()
        {
            return _input.ReadLine();
        }

        private MetaCommandResponse HandleMetaCommand(string input)
        {
            if (input == ".exit")
            {
                return new MetaCommandResponse.Exit(ExitCode.Success);
            }

            if (input == ".begin")
            {
                _output.WriteLine("Beginning transaction");

                _tx = _txm.Begin();
                return new MetaCommandResponse.Success();
            }
            if (input == ".commit")
            {
                _output.WriteLine("Committing transaction");

                _tx.Commit();
                _tx = null;
                return new MetaCommandResponse.Success();
            }
            if (input == ".abort")
            {
                _output.WriteLine("Aborting transaction");

                _tx.Rollback();
                _tx = null;
                return new MetaCommandResponse.Success();
            }
            
            if (input == ".btree")
            {
                var index = _table.Indices.FirstOrDefault();
                if (index != null)
                {
                    MetaCommands.PrintBTree(_output, _pager, _table, index);
                }

                return new MetaCommandResponse.Success();
            }

            return new MetaCommandResponse.Unrecognised(input);
        }

        private ExecuteStatementResponse ExecuteStatement(string input)
        {
            using (var tx = _txm.Begin())
            {
                var result = ExecuteStatement(tx, input);

                if (result is ExecuteStatementResponse.Success _)
                {
                    tx.Commit();
                }

                return result;
            }
        }

        private ExecuteStatementResponse ExecuteStatement(ITransaction tx, string input)
        {
            var parser = new Parser();
            var planner = new Planner(_database);
            var planCompiler = new PlanCompiler(_database);

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
                var executor = new ProgramExecutor(program, _pager, _txm);

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
            _tx?.Dispose();
            _pager?.Dispose();
        }
    }
}
