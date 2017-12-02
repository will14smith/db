using SimpleDatabase.Execution.Values;

namespace SimpleDatabase.Execution
{
    public static class FunctionStateExtensions
    {
        public static (FunctionState, CursorValue) PopCursor(this FunctionState state)
        {
            var (newState, value) = state.PopValue();

            return (newState, (CursorValue)value);
        }
    }
}