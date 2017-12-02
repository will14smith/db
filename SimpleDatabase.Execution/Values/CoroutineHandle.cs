using System.Collections.Generic;

namespace SimpleDatabase.Execution.Values
{
    public class CoroutineHandle : Value
    {
        public FunctionLabel Function { get; }
        public IReadOnlyList<Value> Args { get; }
        public FunctionState State { get; private set; }

        public CoroutineHandle(FunctionLabel function, IReadOnlyList<Value> args, FunctionState state)
        {
            Function = function;
            Args = args;
            State = state;
        }

        public void SetState(FunctionState state)
        {
            // TODO make immutable?
            State = state;
        }
    }
}
