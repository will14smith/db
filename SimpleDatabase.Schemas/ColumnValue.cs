namespace SimpleDatabase.Schemas
{
    public class ColumnValue
    {
        public object Value { get; }

        public ColumnValue(object value)
        {
            Value = value;
        }
    }
}