namespace SimpleDatabase.CLI
{
    public abstract class ExecuteStatementResponse
    {
        public class Success : ExecuteStatementResponse
        {
            public Success()
            {
            }
        }

        public class SyntaxError : ExecuteStatementResponse
        {
            public string Error { get; }

            public SyntaxError(string error)
            {
                Error = error;
            }
        }

        public class Unrecognised : ExecuteStatementResponse
        {
            public string Input { get; }

            public Unrecognised(string input)
            {
                Input = input;
            }
        }
    }
}
