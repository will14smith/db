using SimpleDatabase.Execution.Values;

namespace SimpleDatabase.Execution
{
    public static class ProgramStateExtensions
    {
        public static (ProgramState, CursorValue) PopCursor(this ProgramState state)
        {
            var (newState, value) = state.PopValue();

            return (newState, (CursorValue)value);
        }
    }
}
