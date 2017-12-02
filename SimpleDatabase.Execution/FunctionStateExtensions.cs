using SimpleDatabase.Execution.Values;

namespace SimpleDatabase.Execution
{
    public static class FunctionStateExtensions
    {
        public static (FunctionState, T) PopValue<T>(this FunctionState state)
            where T : Value
        {
            var (newState, value) = state.PopValue();

            return (newState, (T)value);
        }
    }
}