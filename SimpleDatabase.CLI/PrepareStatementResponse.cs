using SimpleDatabase.Core;

namespace SimpleDatabase.CLI
{
    public abstract class PrepareStatementResponse
    {
        public class Success : PrepareStatementResponse
        {
            public IStatement Statement { get; }

            public Success(IStatement statement)
            {
                Statement = statement;
            }
        }

        public class SyntaxError : PrepareStatementResponse
        {
            public string Error { get; }

            public SyntaxError(string error)
            {
                Error = error;
            }
        }

        public class Unrecognised : PrepareStatementResponse
        {
            public string Input { get; }

            public Unrecognised(string input)
            {
                Input = input;
            }
        }
    }
}
