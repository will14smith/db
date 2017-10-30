namespace SimpleDatabase.CLI
{
    public abstract class MetaCommandResponse
    {
        public class Exit : MetaCommandResponse
        {
            public ExitCode Code { get; }

            public Exit(ExitCode code)
            {
                Code = code;
            }
        }

        public class Success : MetaCommandResponse
        {
        }

        public class Unrecognised : MetaCommandResponse
        {
            public string Input { get; }

            public Unrecognised(string input)
            {
                Input = input;
            }
        }
    }
}
