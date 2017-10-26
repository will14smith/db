using System.Text;

namespace SimpleDatabase.CLI.UnitTests
{
    public class FakeREPLOutput : IREPLOutput
    {
        private readonly StringBuilder _sb = new StringBuilder();

        public void Write(string str)
        {
            _sb.Append(str);
        }

        public string Output => _sb.ToString();
    }
}