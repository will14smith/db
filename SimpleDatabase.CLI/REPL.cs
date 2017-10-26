using SimpleDatabase.CLI.MetaCommands;

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
                        case SuccessMetaCommandResponse _:
                            continue;
                        case ExitMetaCommandResponse exit:
                            return exit.Code;
                        case UnrecognisedMetaCommandResponse resp:
                            _output.WriteLine("Unrecognized command '{0}'.", resp.Input);
                            continue;
                        default:
                            _output.WriteLine("Unrecognized meta command response '{0}'.", response.GetType().Name);
                            continue;
                    }
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

        private IMetaCommandResponse HandleMetaCommand(string input)
        {
            if (input == ".exit")
            {
                return new ExitMetaCommandResponse(ExitCode.Success);
            }

            return new UnrecognisedMetaCommandResponse(input);
        }
    }
}
