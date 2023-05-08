namespace SimpleDatabase.CLI.Commands
{
    public abstract class CommandResponse
    {
        public class Exit : CommandResponse
        {
            public ExitCode Code { get; }

            public Exit(ExitCode code)
            {
                Code = code;
            }
        }

        public class Success : CommandResponse
        {
        }

        public class Unrecognised : CommandResponse
        {
            public string Input { get; }

            public Unrecognised(string input)
            {
                Input = input;
            }
        }

        public class Invalid : CommandResponse
        {
            public Invalid(string message)
            {
                Message = message;
            }

            public string Message { get; }
        }
    }
}
