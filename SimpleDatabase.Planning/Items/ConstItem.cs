using System;
using SimpleDatabase.Execution.Operations.Constants;

namespace SimpleDatabase.Planning.Items
{
    public class ConstItem : Item
    {
        private readonly object _value;

        public ConstItem(object value)
        {
            _value = value;
        }

        public override Item Load(IOperationGenerator generator)
        {
            switch (_value)
            {
                case int num:
                    generator.Emit(new ConstIntOperation(num));
                    break;

                case string str:
                    generator.Emit(new ConstStringOperation(str));
                    break;

                default: throw new ArgumentOutOfRangeException(nameof(_value), $"Unhandled type: {_value.GetType().FullName}");
        }

            // TODO type
            return new StackItem();
        }
    }
}
