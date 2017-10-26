using System.Collections.Generic;

namespace SimpleDatabase.CLI.UnitTests
{
    public class FakeREPLInput : IREPLInput
    {
        private readonly IREPLOutput _output;
        private readonly IReadOnlyList<string> _inputs;
        private int _index;

        public FakeREPLInput(IREPLOutput output, IReadOnlyList<string> inputs)
        {
            _output = output;
            _inputs = inputs;
        }

        public string ReadLine()
        {
            var input = _inputs[_index++];

            _output.WriteLine(input);

            return input;
        }
    }
}