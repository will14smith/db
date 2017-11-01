using SimpleDatabase.Core;
using SimpleDatabase.Core.Paging;

namespace SimpleDatabase.CLI
{
    public class REPL
    {
        private readonly IREPLInput _input;
        private readonly IREPLOutput _output;

        private readonly Pager _pager;
        private readonly Table _table;

        public REPL(IREPLInput input, IREPLOutput output, string file)
        {
            _input = input;
            _output = output;

            var storage = new FilePagerStorage(file);
            _pager = new Pager(storage);
            _table = new Table(_pager);
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

                var statementResponse = PrepareStatement(line);
                switch (statementResponse)
                {
                    case PrepareStatementResponse.Success resp:
                        ExecuteStatement(resp.Statement);
                        break;

                    case PrepareStatementResponse.SyntaxError resp:
                        _output.WriteLine("Syntax error '{0}'.", resp.Error);
                        break;

                    case PrepareStatementResponse.Unrecognised resp:
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
                _table.Dispose();
                return new MetaCommandResponse.Exit(ExitCode.Success);
            }
            if (input == ".constants")
            {
                MetaCommands.PrintConstants(_output);
                return new MetaCommandResponse.Success();
            }
            if (input == ".btree")
            {
                MetaCommands.PrintBTree(_output, _pager, _table.RootPageNumber);
                return new MetaCommandResponse.Success();
            }

            return new MetaCommandResponse.Unrecognised(input);
        }

        private PrepareStatementResponse PrepareStatement(string input)
        {
            if (input.StartsWith("insert"))
            {
                var tokens = input.Split(" ");
                if (tokens.Length != 4)
                {
                    return new PrepareStatementResponse.SyntaxError("Expected 3 parameters (id, username, email) for insert");
                }

                var id = int.Parse(tokens[1]);
                var username = tokens[2];
                var email = tokens[3];

                var row = new Row(id, username, email);

                return new PrepareStatementResponse.Success(new InsertStatement(row));
            }
            if (input.StartsWith("select"))
            {
                return new PrepareStatementResponse.Success(new SelectStatement());
            }

            return new PrepareStatementResponse.Unrecognised(input);
        }

        private void ExecuteStatement(IStatement statement)
        {
            switch (statement)
            {
                case InsertStatement insert:
                    ExecuteInsert(insert);
                    break;
                case SelectStatement select:
                    ExecuteSelect(select);
                    break;
            }
        }

        private void ExecuteInsert(InsertStatement insert)
        {
            var result = _table.Insert(insert);
            switch (result)
            {
                case InsertResult.Success _:
                    _output.WriteLine("Executed.");
                    break;
                case InsertResult.TableFull _:
                    _output.WriteLine("Error: Table full.");
                    break;
            }
        }

        private void ExecuteSelect(SelectStatement select)
        {
            var result = _table.Select(select);
            switch (result)
            {
                case SelectResult.Success success:
                    foreach (var row in success.Rows)
                    {
                        _output.WriteLine(row.ToString());
                    }

                    _output.WriteLine("Executed.");
                    break;
            }

            _table.Select(select);
        }
    }
}
