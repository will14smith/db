using System.Collections.Generic;

namespace SimpleDatabase.CLI.UnitTests
{
    public class FakeREPLInput : IREPLInput
    {
        private readonly IReadOnlyList<string> _inputs;
        private int _index;

        public FakeREPLInput(IReadOnlyList<string> inputs)
        {
            _inputs = inputs;
        }

        public string ReadLine()
        {
            var input = _inputs[_index++];

            return input;
        }
    }
}