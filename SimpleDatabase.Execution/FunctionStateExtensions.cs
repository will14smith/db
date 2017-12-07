using System;

namespace SimpleDatabase.Execution
{
    public static class FunctionStateExtensions
    {
        public static (FunctionState, T) PopValue<T>(this FunctionState state)
            where T : class
        {
            var (newState, value) = state.PopValue();

            if (!(value is T t))
            {
                throw new InvalidCastException($"Cannot cast {value?.GetType()} to {typeof(T)}");
            }

            return (newState, t);
        }
    }
}