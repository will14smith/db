namespace SimpleDatabase.Core.Execution.Values
{
    public class ObjectValue : Value
    {
        public object Value { get; }

        public ObjectValue(object value)
        {
            Value = value;
        }
    }
}
