using SimpleDatabase.CLI.MetaCommands;
using SimpleDatabase.CLI.PrepareStatementResponses;
using SimpleDatabase.Core;

namespace SimpleDatabase.CLI
{
    public class REPL
    {
        private readonly IREPLInput _input;
        private readonly IREPLOutput _output;

        public REPL(IREPLInput input, IREPLOutput output)
        {
            _input = input;
            _output = output;
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
                        _output.WriteLine("Executed.");
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
                return new MetaCommandResponse.Exit(ExitCode.Success);
            }

            return new MetaCommandResponse.Unrecognised(input);
        }

        private PrepareStatementResponse PrepareStatement(string input)
        {
            if (input.StartsWith("insert"))
            {
                return new SuccessPrepareStatementResponses(new InsertStatement());
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
                    _output.WriteLine("This is where we would do an insert.");
                    break;
                case SelectStatement select:
                    _output.WriteLine("This is where we would do an select.");
                    break;
            }
        }
    }
}
