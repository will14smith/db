using System.Collections.Generic;
using System.Text;
using SimpleDatabase.Execution.Operations;

namespace SimpleDatabase.Execution
{
    public class Function
    {
        public IReadOnlyList<IOperation> Operations { get; }
        public IReadOnlyDictionary<SlotLabel, SlotDefinition> Slots { get; }

        public Function(IReadOnlyList<IOperation> operations, IReadOnlyDictionary<SlotLabel, SlotDefinition> slots)
        {
            Operations = operations;
            Slots = slots;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("[");

            foreach (var kvp in Slots)
            {
                sb.AppendLine("  " + kvp.Value);
            }

            sb.AppendLine("] {");

            foreach (var op in Operations)
            {
                if (op is ProgramLabel)
                {
                    sb.AppendLine(op + ":");
                }
                else
                {
                    sb.AppendLine("  " + op);
                }
            }

            sb.Append("}");

            return sb.ToString();
        }
    }
}
