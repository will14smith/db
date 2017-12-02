namespace SimpleDatabase.Execution.Values
{
    public class ObjectValue : Value
    {
        public object Value { get; }

        public ObjectValue(object value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value?.ToString();
        }
    }
}
