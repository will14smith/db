using System;
using SimpleDatabase.Execution.Trees;
using SimpleDatabase.Parsing.Statements;
using SimpleDatabase.Schemas;
using SimpleDatabase.Schemas.Types;
using SimpleDatabase.Storage;
using SimpleDatabase.Storage.Nodes;
using SimpleDatabase.Storage.Paging;
using SimpleDatabase.Storage.Serialization;
using Table = SimpleDatabase.Schemas.Table;

namespace SimpleDatabase.CLI
{
    public class REPL : IDisposable
    {
        private readonly IREPLInput _input;
        private readonly IREPLOutput _output;

        private readonly Pager _pager;
        private readonly StoredTable _table;

        public REPL(IREPLInput input, IREPLOutput output, string file)
        {
            _input = input;
            _output = output;

            var storage = new FilePagerStorage(file);
            _pager = new Pager(storage);

            _table = CreateTable("table", new[]
            {
                new Column("id", new ColumnType.Integer()),
                new Column("name", new ColumnType.String(31)),
                new Column("email", new ColumnType.String(255)),
            });
        }

        private StoredTable CreateTable(string name, Column[] columns)
        {
            var table = new Table(name, columns);

            var rootPage = _pager.Allocate();
            var rowSerializer = CreateRowSerializer(table);
            var node = LeafNode.New(rowSerializer, rootPage);
            node.IsRoot = true;
            _pager.Flush(rootPage.Number);

            return new StoredTable(table, rootPage.Number);
        }
        private static RowSerializer CreateRowSerializer(Table table)
        {
            return new RowSerializer(table, new ColumnTypeSerializerFactory());
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
                MetaCommands.PrintBTree(_output, _pager, _table);
                return new MetaCommandResponse.Success();
            }

            return new MetaCommandResponse.Unrecognised(input);
        }

        private ExecuteStatementResponse ExecuteStatement(string input)
        {
            if (input.StartsWith("insert"))
            {
                var tokens = input.Split(" ");
                if (tokens.Length != 4)
                {
                    return new ExecuteStatementResponse.SyntaxError("Expected 3 parameters (id, username, email) for insert");
                }

                var id = int.Parse(tokens[1]);
                var username = tokens[2];
                var email = tokens[3];

                var result = new TreeInserter(_pager, CreateRowSerializer(_table.Table), _table).Insert(id, new Row(new[]
                {
                    new ColumnValue(id),
                    new ColumnValue(username),
                    new ColumnValue(email)
                }));

                switch (result)
                {
                    case TreeInsertResult.Success _:
                        _output.WriteLine("Executed.");
                        break;
                    case TreeInsertResult.DuplicateKey _:
                        _output.WriteLine("Error: Duplicate key.");
                        break;

                    default:
                        _output.WriteLine($"Unhandled InsertResult: {result}");
                        break;
                }


                return new ExecuteStatementResponse.Success();
            }

            if (input.StartsWith("delete"))
            {
                var tokens = input.Split(" ");
                if (tokens.Length != 2)
                {
                    ;
                    return new ExecuteStatementResponse.SyntaxError("Expected 1 parameter (id) for delete");
                }

                var id = int.Parse(tokens[1]);

                var result = new TreeDeleter(_pager, CreateRowSerializer(_table.Table), _table).Delete(id);

                switch (result)
                {
                    case TreeDeleteResult.Success _:
                        _output.WriteLine("Executed.");
                        break;
                    case TreeDeleteResult.KeyNotFound knf:
                        _output.WriteLine($"Error: Key not found {knf.Key}.");
                        break;

                    default:
                        _output.WriteLine($"Unhandled InsertResult: {result}");
                        break;
                }


                return new ExecuteStatementResponse.Success();
            }

            throw new NotImplementedException("use new pipeline");
        }

        public void Dispose()
        {
            _pager?.Dispose();
        }
    }
}
