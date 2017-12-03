using SimpleDatabase.Schemas;

namespace SimpleDatabase.Execution.Values
{
    public class ObjectValue : Value
    {
        public object Value { get; }

        public ObjectValue(object value)
        {
            switch (value)
            {
                case ObjectValue obj:
                    Value = obj.Value;
                    break;
                case ColumnValue col:
                    Value = col.Value;
                    break;
                default:
                    Value = value;
                    break;
            }
        }

        public override string ToString()
        {
            return Value?.ToString();
        }
    }
}
