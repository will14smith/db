using System.Collections.Generic;
using System.Linq;
using SimpleDatabase.Execution;

namespace SimpleDatabase.Planning
{
    internal class ProgramGenerator
    {
        private readonly List<OperationGenerator> _functions = new List<OperationGenerator>();

        public IOperationGenerator NewFunction()
        {
            var label = FunctionLabel.Create();
            var generator = new OperationGenerator(this, label);

            _functions.Add(generator);

            return generator;
        }

        public Program CreateProgram(IOperationGenerator entry)
        {
            return new Program(entry.Label, _functions.ToDictionary(x => x.Label, x => x.CreateFunction()));
        }
    }
}