﻿using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleDatabase.Execution
{
    public class Program
    {
        public FunctionLabel Entry { get; }
        public IReadOnlyDictionary<FunctionLabel, Function> Functions { get; }

        public Program(FunctionLabel entry, IReadOnlyDictionary<FunctionLabel, Function> functions)
        {
            Entry = entry;
            Functions = functions;
        }

        public override string ToString()
        {
            var entryFunction = Functions[Entry];
            var otherFunctions = Functions.Where(x => x.Key != Entry);

            var sb = new StringBuilder();

            sb.AppendLine(entryFunction.ToString());

            foreach (var kvp in otherFunctions)
            {
                sb.AppendLine();
                sb.AppendLine("func " + kvp.Key + ":");
                sb.AppendLine(kvp.Value.ToString());
            }

            return sb.ToString();
        }
    }
}
