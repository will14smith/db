using System;
using SimpleDatabase.Execution.Operations.Constants;

namespace SimpleDatabase.Planning.Items
{
    public class ConstItem : Item
    {
        private readonly object _value;

        public ConstItem(IOperationGenerator generator, object value) : base(generator)
        {
            _value = value;
        }

        public override Item Load()
        {
            switch (_value)
            {
                case int num:
                    Generator.Emit(new ConstIntOperation(num));
                    break;

                case string str:
                    Generator.Emit(new ConstStringOperation(str));
                    break;

                default: throw new ArgumentOutOfRangeException(nameof(_value), $"Unhandled type: {_value.GetType().FullName}");
        }

            // TODO type
            return new StackItem(Generator);
        }
    }
}
