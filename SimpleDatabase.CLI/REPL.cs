using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;
using SimpleDatabase.Execution;
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
            });
        }

        private Table CreateTable(string name, Column[] columns)
        {
            var table = new Table(name, columns, new Index[0]);

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

                var statementResponse = ExecuteStatement(line);
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
            var database = new Database(new[] { _table });

            var parser = new Parser();
            IReadOnlyCollection<Statement> statements;
            try
            {
                statements = parser.Parse(input);
            }
            catch (ParseCanceledException ex)
            {
                return new ExecuteStatementResponse.SyntaxError(ex.Message);
            }

            var planner = new Planner(database);
            var plans = statements.Select(planner.Plan);

            var planCompiler = new PlanCompiler(database);
            var programs = plans.Select(planCompiler.Compile);

            foreach (var program in programs)
            {
                var executor = new ProgramExecutor(program, _pager);

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
            _pager?.Dispose();
        }
    }
}
