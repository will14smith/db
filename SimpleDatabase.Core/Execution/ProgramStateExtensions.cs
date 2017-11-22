using SimpleDatabase.Core.Execution.Values;

namespace SimpleDatabase.Core.Execution
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
